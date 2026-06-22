using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

/// <summary>
/// Command for cancelling an item from a sale.
/// </summary>
public record CancelSaleItemCommand : IRequest<CancelSaleItemResponse>
{
    /// <summary>
    /// Gets the unique identifier of the sale.
    /// </summary>
    public Guid SaleId { get; }

    /// <summary>
    /// Gets the unique identifier of the item to cancel.
    /// </summary>
    public Guid ItemId { get; }

    /// <summary>
    /// Initializes a new instance of CancelSaleItemCommand.
    /// </summary>
    /// <param name="saleId">The sale ID.</param>
    /// <param name="itemId">The item ID.</param>
    public CancelSaleItemCommand(Guid saleId, Guid itemId)
    {
        SaleId = saleId;
        ItemId = itemId;
    }
}
