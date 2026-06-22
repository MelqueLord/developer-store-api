using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<UpdateSaleHandler>>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given existing sale When updating sale Then replaces items and saves sale")]
    public async Task Handle_ExistingSale_ReplacesItemsAndSavesSale()
    {
        var sale = CreateSale();
        var command = CreateValidCommand(sale.Id);
        var updatedItems = CreateItems(command);
        var result = new UpdateSaleResult { Id = sale.Id, TotalAmount = 80m };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<List<SaleItem>>(command.Items).Returns(updatedItems);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());
        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>()).Returns(result);

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().NotBeNull();
        response.Id.Should().Be(sale.Id);
        response.TotalAmount.Should().Be(80m);
        sale.TotalAmount.Should().Be(80m);
        sale.Items.Should().HaveCount(1);
        sale.DomainEvents.Should().Contain(domainEvent => domainEvent is SaleModifiedEvent);

        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(updatedSale => updatedSale.TotalAmount == 80m),
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

    private static List<SaleItem> CreateItems(UpdateSaleCommand command)
    {
        return command.Items
            .Select(item => new SaleItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            })
            .ToList();
    }
}
