using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Mapping;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly IMapper _mapper;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _logger = Substitute.For<ILogger<UpdateSaleHandler>>();
        _mapper = MapperFactory.Create();
        _handler = new UpdateSaleHandler(_saleRepository, _logger, _mapper);
    }

    [Fact(DisplayName = "Given existing sale When updating sale Then replaces items and saves sale")]
    public async Task Handle_ExistingSale_ReplacesItemsAndSavesSale()
    {
        var sale = CreateSale();
        var command = CreateValidCommand(sale.Id);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().NotBeNull();
        response.Id.Should().Be(sale.Id);
        response.SaleNumber.Should().Be("SALE-UPDATED");
        response.TotalAmount.Should().Be(80m);
        response.Items.Should().ContainSingle();
        response.Items[0].DiscountPercentage.Should().Be(0.20m);
        sale.TotalAmount.Should().Be(80m);
        sale.Items.Should().HaveCount(1);
        sale.DomainEvents.Should().Contain(domainEvent => domainEvent is SaleModifiedEvent);

        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(updatedSale => updatedSale.TotalAmount == 80m),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given missing sale When updating sale Then throws KeyNotFoundException")]
    public async Task Handle_MissingSale_ThrowsKeyNotFoundException()
    {
        var saleId = Guid.NewGuid();
        var command = CreateValidCommand(saleId);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {saleId} not found");

        await _saleRepository.DidNotReceive().UpdateAsync(
            Arg.Any<Sale>(),
            Arg.Any<CancellationToken>());
    }

    private static UpdateSaleCommand CreateValidCommand(Guid saleId)
    {
        return new UpdateSaleCommand
        {
            Id = saleId,
            SaleNumber = "SALE-UPDATED",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Updated customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Updated branch",
            Items =
            [
                new UpdateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Updated product",
                    Quantity = 10,
                    UnitPrice = 10m
                }
            ]
        };
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
                    ProductName = "Old product",
                    Quantity = 1,
                    UnitPrice = 10m
                }
            ]
        };
    }
}
