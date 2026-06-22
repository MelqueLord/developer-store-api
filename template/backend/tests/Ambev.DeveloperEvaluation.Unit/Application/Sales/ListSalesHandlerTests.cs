using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class ListSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ListSalesHandler _handler;

    public ListSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ListSalesHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given existing sales When listing sales Then returns mapped sales")]
    public async Task Handle_ExistingSales_ReturnsMappedSales()
    {
        var sales = new List<Sale>
        {
            CreateSale("SALE-001"),
            CreateSale("SALE-002")
        };

        var results = sales
            .Select(sale => new ListSalesResult
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                TotalAmount = sale.TotalAmount
            })
            .ToList();

        _saleRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(sales);
        _mapper.Map<IEnumerable<ListSalesResult>>(sales).Returns(results);

        var response = await _handler.Handle(new ListSalesCommand(), CancellationToken.None);

        response.Should().HaveCount(2);
        response.Select(sale => sale.SaleNumber).Should().Contain(["SALE-001", "SALE-002"]);

        await _saleRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    private static Sale CreateSale(string saleNumber)
    {
        return new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = saleNumber,
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Test customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Test branch",
            TotalAmount = 36m
        };
    }
}
