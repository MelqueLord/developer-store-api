using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }

    public void ApplyBusinessRules()
    {
        if (ProductId == Guid.Empty)
            throw new DomainException("Product is required");

        if (string.IsNullOrWhiteSpace(ProductName))
            throw new DomainException("Product name is required");

        if (Quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        if (Quantity > 20)
            throw new DomainException("It is not possible to sell more than 20 identical items");

        if (UnitPrice <= 0)
            throw new DomainException("Unit price must be greater than zero");

        DiscountPercentage = Quantity switch
        {
            >= 10 => 0.20m,
            >= 4 => 0.10m,
            _ => 0m
        };

        var grossAmount = Quantity * UnitPrice;
        DiscountAmount = grossAmount * DiscountPercentage;
        TotalAmount = grossAmount - DiscountAmount;
    }

    public void Cancel()
    {
        IsCancelled = true;
    }
}
