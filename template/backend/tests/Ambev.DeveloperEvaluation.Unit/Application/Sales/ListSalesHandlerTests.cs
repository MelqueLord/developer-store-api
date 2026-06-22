using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Mapping;
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
        _mapper = MapperFactory.Create();
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

        var command = new ListSalesCommand
        {
            Page = 2,
            Size = 5,
            Order = "saleNumber asc",
            CustomerName = "Test*",
            IsCancelled = false
        };

        _saleRepository.GetPagedAsync(
                command.Page,
                command.Size,
                command.Order,
                command.SaleNumber,
                command.CustomerName,
                command.BranchName,
                command.IsCancelled,
                command.MinSaleDate,
                command.MaxSaleDate,
                command.MinTotalAmount,
                command.MaxTotalAmount,
                Arg.Any<CancellationToken>())
            .Returns(new PagedResult<Sale>(sales, 12, command.Page, command.Size));

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Items.Should().HaveCount(2);
        response.Items.Select(sale => sale.SaleNumber).Should().Contain(["SALE-001", "SALE-002"]);
        response.CurrentPage.Should().Be(2);
        response.PageSize.Should().Be(5);
        response.TotalPages.Should().Be(3);
        response.TotalCount.Should().Be(12);

        await _saleRepository.Received(1).GetPagedAsync(
            command.Page,
            command.Size,
            command.Order,
            command.SaleNumber,
            command.CustomerName,
            command.BranchName,
            command.IsCancelled,
            command.MinSaleDate,
            command.MaxSaleDate,
            command.MinTotalAmount,
            command.MaxTotalAmount,
            Arg.Any<CancellationToken>());
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
