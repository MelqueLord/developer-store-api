using MediatR;
using Ambev.DeveloperEvaluation.Common.Pagination;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Command for retrieving sales with pagination, ordering and filters.
/// </summary>
public record ListSalesCommand : IRequest<PagedResult<ListSalesResult>>
{
    public int Page { get; init; } = 1;
    public int Size { get; init; } = 10;
    public string? Order { get; init; }
    public string? SaleNumber { get; init; }
    public string? CustomerName { get; init; }
    public string? BranchName { get; init; }
    public bool? IsCancelled { get; init; }
    public DateTime? MinSaleDate { get; init; }
    public DateTime? MaxSaleDate { get; init; }
    public decimal? MinTotalAmount { get; init; }
    public decimal? MaxTotalAmount { get; init; }
}
