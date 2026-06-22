using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// Profile for mapping between Application and API ListSales models.
/// </summary>
public class ListSalesProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for ListSales feature.
    /// </summary>
    public ListSalesProfile()
    {
        CreateMap<ListSalesResult, ListSalesResponse>();
        CreateMap<GetSaleItemResult, GetSaleItemResponse>();
    }
}
