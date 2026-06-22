using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CancelSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<CancelSaleItemHandler> _logger;
    private readonly CancelSaleItemHandler _handler;

    public CancelSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _logger = Substitute.For<ILogger<CancelSaleItemHandler>>();
        _handler = new CancelSaleItemHandler(_saleRepository, _logger);
    }

    [Fact(DisplayName = "Given existing sale item When cancelling item Then cancels item and saves sale")]
    public async Task Handle_ExistingSaleItem_CancelsItemAndSavesSale()
    {
        var sale = CreateSale();
        var item = sale.Items.First();
        var command = new CancelSaleItemCommand(sale.Id, item.Id);

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Success.Should().BeTrue();
        item.IsCancelled.Should().BeTrue();
        sale.TotalAmount.Should().Be(36m);
        sale.DomainEvents.Should().Contain(domainEvent => domainEvent is ItemCancelledEvent);

        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(updatedSale => updatedSale.Id == sale.Id && updatedSale.TotalAmount == 36m),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given missing sale When cancelling item Then throws KeyNotFoundException")]
    public async Task Handle_MissingSale_ThrowsKeyNotFoundException()
    {
        var saleId = Guid.NewGuid();
        var command = new CancelSaleItemCommand(saleId, Guid.NewGuid());

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {saleId} not found");

        await _saleRepository.DidNotReceive().UpdateAsync(
            Arg.Any<Sale>(),
            Arg.Any<CancellationToken>());
    }

    private static Sale CreateSale()
    {
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Test customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Test branch",
            Items =
            [
                new SaleItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    ProductName = "Cancelled product",
                    Quantity = 3,
                    UnitPrice = 10m
                },
                new SaleItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    ProductName = "Active product",
                    Quantity = 4,
                    UnitPrice = 10m
                }
            ]
        };

        sale.Recalculate();
        return sale;
    }
}
