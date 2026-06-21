using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Handler for processing ListSalesCommand requests.
/// </summary>
public class ListSalesHandler : IRequestHandler<ListSalesCommand, IEnumerable<ListSalesResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of ListSalesHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the ListSalesCommand request.
    /// </summary>
    /// <param name="request">The ListSales command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sales list.</returns>
    public async Task<IEnumerable<ListSalesResult>> Handle(ListSalesCommand request, CancellationToken cancellationToken)
    {
        var sales = await _saleRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ListSalesResult>>(sales);
    }
}
