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
}
