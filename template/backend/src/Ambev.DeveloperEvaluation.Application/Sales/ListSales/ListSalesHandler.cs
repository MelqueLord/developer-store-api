using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Handler for processing ListSalesCommand requests.
/// </summary>
public class ListSalesHandler : IRequestHandler<ListSalesCommand, PagedResult<ListSalesResult>>
{
    private readonly ISaleRepository _saleRepository;

    /// <summary>
    /// Initializes a new instance of ListSalesHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    public ListSalesHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    /// <summary>
    /// Handles the ListSalesCommand request.
    /// </summary>
    /// <param name="request">The ListSales command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The paginated sales list.</returns>
    public async Task<PagedResult<ListSalesResult>> Handle(ListSalesCommand request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var size = Math.Clamp(request.Size, 1, 100);

        var sales = await _saleRepository.GetPagedAsync(
            page,
            size,
            request.Order,
            request.SaleNumber,
            request.CustomerName,
            request.BranchName,
            request.IsCancelled,
            request.MinSaleDate,
            request.MaxSaleDate,
            request.MinTotalAmount,
            request.MaxTotalAmount,
            cancellationToken);

        var results = sales.Items.Select(sale => sale.ToListSalesResult()).ToList();
        return new PagedResult<ListSalesResult>(results, sales.TotalCount, sales.CurrentPage, sales.PageSize);
    }
}
