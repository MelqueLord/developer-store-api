using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

/// <summary>
/// Handler for processing CancelSaleItemCommand requests.
/// </summary>
public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<CancelSaleItemHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CancelSaleItemHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="logger">The application logger.</param>
    public CancelSaleItemHandler(ISaleRepository saleRepository, ILogger<CancelSaleItemHandler> logger)
    {
        _saleRepository = saleRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CancelSaleItemCommand request.
    /// </summary>
    /// <param name="request">The CancelSaleItem command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the cancel item operation.</returns>
    public async Task<CancelSaleItemResponse> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found");

        sale.CancelItem(request.ItemId);

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        _logger.LogInformation("ItemCancelled event: {SaleId} {ItemId}", sale.Id, request.ItemId);

        return new CancelSaleItemResponse { Success = true };
    }
}
