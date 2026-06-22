using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact(DisplayName = "Sale should calculate total from active items")]
    public void Given_SaleWithItems_When_Recalculate_Then_ShouldUpdateTotalAmount()
    {
        var sale = CreateSale();
        sale.Items.Add(CreateItem(quantity: 3, unitPrice: 10m));
        sale.Items.Add(CreateItem(quantity: 10, unitPrice: 5m));

        sale.Recalculate();

        Assert.Equal(70m, sale.TotalAmount);
    }

    [Fact(DisplayName = "Sale should not include cancelled items in total")]
    public void Given_SaleWithCancelledItem_When_Recalculate_Then_ShouldIgnoreCancelledItem()
    {
        var cancelledItem = CreateItem(quantity: 3, unitPrice: 10m);
        cancelledItem.Cancel();

        var sale = CreateSale();
        sale.Items.Add(cancelledItem);
        sale.Items.Add(CreateItem(quantity: 4, unitPrice: 10m));

        sale.Recalculate();

        Assert.Equal(36m, sale.TotalAmount);
    }

    [Fact(DisplayName = "Sale cancellation should cancel all items")]
    public void Given_SaleWithItems_When_Cancel_Then_ShouldCancelSaleAndItems()
    {
        var sale = CreateSale();
        sale.Items.Add(CreateItem(quantity: 3, unitPrice: 10m));
        sale.Items.Add(CreateItem(quantity: 4, unitPrice: 10m));

        sale.Cancel();

        Assert.True(sale.IsCancelled);
        Assert.All(sale.Items, item => Assert.True(item.IsCancelled));
        Assert.NotNull(sale.UpdatedAt);
    }

    [Fact(DisplayName = "Cancelled sale should not allow item replacement")]
    public void Given_CancelledSale_When_ReplaceItems_Then_ShouldThrowDomainException()
    {
        var sale = CreateSale();
        sale.Cancel();

        Assert.Throws<DomainException>(() => sale.ReplaceItems([CreateItem(quantity: 1, unitPrice: 10m)]));
    }

    [Fact(DisplayName = "Created sale should register SaleCreated event")]
    public void Given_Sale_When_RegisterCreated_Then_ShouldAddSaleCreatedEvent()
    {
        var sale = CreateSale();

        sale.RegisterCreated();

        Assert.Contains(sale.DomainEvents, domainEvent => domainEvent is SaleCreatedEvent);
    }

    [Fact(DisplayName = "Modified sale should register SaleModified event")]
    public void Given_Sale_When_ReplaceItems_Then_ShouldAddSaleModifiedEvent()
    {
        var sale = CreateSale();

        sale.ReplaceItems([CreateItem(quantity: 4, unitPrice: 10m)]);

        Assert.Contains(sale.DomainEvents, domainEvent => domainEvent is SaleModifiedEvent);
    }

    [Fact(DisplayName = "Cancelled sale should register cancellation events")]
    public void Given_SaleWithItems_When_Cancel_Then_ShouldAddCancellationEvents()
    {
        var sale = CreateSale();
        sale.Items.Add(CreateItem(quantity: 3, unitPrice: 10m));

        sale.Cancel();

        Assert.Contains(sale.DomainEvents, domainEvent => domainEvent is SaleCancelledEvent);
        Assert.Contains(sale.DomainEvents, domainEvent => domainEvent is ItemCancelledEvent);
    }

    private static Sale CreateSale()
    {
        return new Sale
        {
            SaleNumber = "SALE-001",
            CustomerId = Guid.NewGuid(),
            CustomerName = "Test customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Test branch"
        };
    }

    private static SaleItem CreateItem(int quantity, decimal unitPrice)
    {
        return new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Test product",
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }
}
