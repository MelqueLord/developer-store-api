using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _handler = new GetSaleHandler(_saleRepository);
    }

    [Fact(DisplayName = "Given existing sale When getting sale Then returns sale")]
    public async Task Handle_ExistingSale_ReturnsSale()
    {
        var sale = CreateSale();
        var command = new GetSaleCommand(sale.Id);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().NotBeNull();
        response.Id.Should().Be(sale.Id);
        response.SaleNumber.Should().Be(sale.SaleNumber);
        response.TotalAmount.Should().Be(sale.TotalAmount);

        await _saleRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given missing sale When getting sale Then throws KeyNotFoundException")]
    public async Task Handle_MissingSale_ThrowsKeyNotFoundException()
    {
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {saleId} not found");
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
            TotalAmount = 36m
        };
    }
}
