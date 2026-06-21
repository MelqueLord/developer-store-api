using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Command for retrieving all sales.
/// </summary>
public record ListSalesCommand : IRequest<IEnumerable<ListSalesResult>>;
