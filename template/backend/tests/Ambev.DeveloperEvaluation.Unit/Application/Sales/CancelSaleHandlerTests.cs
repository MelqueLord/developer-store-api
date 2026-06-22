using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<CancelSaleHandler> _logger;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _logger = Substitute.For<ILogger<CancelSaleHandler>>();
        _handler = new CancelSaleHandler(_saleRepository, _logger);
    }

    [Fact(DisplayName = "Given existing sale When cancelling sale Then cancels sale and saves it")]
    public async Task Handle_ExistingSale_CancelsSaleAndSavesIt()
    {
        var sale = CreateSale();
        var command = new CancelSaleCommand(sale.Id);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Success.Should().BeTrue();
        sale.IsCancelled.Should().BeTrue();
        sale.Items.Should().OnlyContain(item => item.IsCancelled);
        sale.DomainEvents.Should().Contain(domainEvent => domainEvent is SaleCancelledEvent);
        sale.DomainEvents.Should().Contain(domainEvent => domainEvent is ItemCancelledEvent);

        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(updatedSale => updatedSale.IsCancelled),
            Arg.Any<CancellationToken>());
    }

    private static Sale CreateSale()
    {
        return new Sale
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
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test product",
                    Quantity = 1,
                    UnitPrice = 10m
                }
            ]
        };
    }
}
