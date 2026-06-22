namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// Represents a request to retrieve a sale by ID.
/// </summary>
public class GetSaleRequest
{
    /// <summary>
    /// Gets or sets the sale identifier.
    /// </summary>
    public Guid Id { get; set; }
}
