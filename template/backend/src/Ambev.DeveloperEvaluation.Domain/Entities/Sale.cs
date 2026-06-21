using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();

    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
        SaleDate = DateTime.UtcNow;
    }

    public void Recalculate()
    {
        ValidateSaleData();

        if (Items.Count == 0)
            throw new DomainException("Sale must have at least one item");

        foreach (var item in Items)
            item.ApplyBusinessRules();

        TotalAmount = Items
            .Where(item => !item.IsCancelled)
            .Sum(item => item.TotalAmount);
    }

    public void ReplaceItems(IEnumerable<SaleItem> items)
    {
        if (IsCancelled)
            throw new DomainException("Cannot change a cancelled sale");

        Items = items.ToList();
        Recalculate();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;

        foreach (var item in Items)
            item.Cancel();
    }

    private void ValidateSaleData()
    {
        if (string.IsNullOrWhiteSpace(SaleNumber))
            throw new DomainException("Sale number is required");

        if (CustomerId == Guid.Empty)
            throw new DomainException("Customer is required");

        if (string.IsNullOrWhiteSpace(CustomerName))
            throw new DomainException("Customer name is required");

        if (BranchId == Guid.Empty)
            throw new DomainException("Branch is required");

        if (string.IsNullOrWhiteSpace(BranchName))
            throw new DomainException("Branch name is required");
    }
}
