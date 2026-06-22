using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _logger = Substitute.For<ILogger<CreateSaleHandler>>();
        _handler = new CreateSaleHandler(_saleRepository, _logger);
    }

    [Fact(DisplayName = "Given valid sale data When creating sale Then recalculates and saves sale")]
    public async Task Handle_ValidRequest_RecalculatesAndSavesSale()
    {
        var command = CreateValidCommand();
        var saleId = Guid.NewGuid();

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var sale = callInfo.Arg<Sale>();
                sale.Id = saleId;
                return sale;
            });

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().NotBeNull();
        response.Id.Should().Be(saleId);
        response.SaleNumber.Should().Be("SALE-001");
        response.TotalAmount.Should().Be(36m);
        response.Items.Should().ContainSingle();
        response.Items[0].DiscountAmount.Should().Be(4m);
        response.Items[0].DiscountPercentage.Should().Be(0.10m);

        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(createdSale =>
                createdSale.TotalAmount == 36m &&
                createdSale.DomainEvents.Any(domainEvent => domainEvent is SaleCreatedEvent)),
            Arg.Any<CancellationToken>());
    }

    private static CreateSaleCommand CreateValidCommand()
    {
        return new CreateSaleCommand
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Test customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Test branch",
            Items =
            [
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test product",
                    Quantity = 4,
                    UnitPrice = 10m
                }
            ]
        };
    }
}
