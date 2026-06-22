using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CreateSaleHandler>>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid sale data When creating sale Then recalculates and saves sale")]
    public async Task Handle_ValidRequest_RecalculatesAndSavesSale()
    {
        var command = CreateValidCommand();
        var sale = CreateSale(command);
        var result = new CreateSaleResult { Id = sale.Id, TotalAmount = 36m };

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(result);

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().NotBeNull();
        response.Id.Should().Be(sale.Id);
        response.TotalAmount.Should().Be(36m);
        sale.TotalAmount.Should().Be(36m);
        sale.DomainEvents.Should().Contain(domainEvent => domainEvent is SaleCreatedEvent);

        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(createdSale => createdSale.TotalAmount == 36m),
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

    private static Sale CreateSale(CreateSaleCommand command)
    {
        return new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = command.SaleNumber,
            SaleDate = command.SaleDate,
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            Items = command.Items
                .Select(item => new SaleItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                })
                .ToList()
        };
    }
}
