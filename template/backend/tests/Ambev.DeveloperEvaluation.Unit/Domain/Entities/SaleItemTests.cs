using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleItemTests
{
    [Fact(DisplayName = "Item below 4 units should not receive discount")]
    public void Given_ItemBelowFourUnits_When_ApplyBusinessRules_Then_ShouldNotApplyDiscount()
    {
        var item = CreateItem(quantity: 3, unitPrice: 10m);

        item.ApplyBusinessRules();

        Assert.Equal(0m, item.DiscountPercentage);
        Assert.Equal(0m, item.DiscountAmount);
        Assert.Equal(30m, item.TotalAmount);
    }

    [Fact(DisplayName = "Item with 4 to 9 units should receive 10 percent discount")]
    public void Given_ItemBetweenFourAndNineUnits_When_ApplyBusinessRules_Then_ShouldApplyTenPercentDiscount()
    {
        var item = CreateItem(quantity: 4, unitPrice: 10m);

        item.ApplyBusinessRules();

        Assert.Equal(0.10m, item.DiscountPercentage);
        Assert.Equal(4m, item.DiscountAmount);
        Assert.Equal(36m, item.TotalAmount);
    }

    [Fact(DisplayName = "Item with 10 to 20 units should receive 20 percent discount")]
    public void Given_ItemBetweenTenAndTwentyUnits_When_ApplyBusinessRules_Then_ShouldApplyTwentyPercentDiscount()
    {
        var item = CreateItem(quantity: 10, unitPrice: 10m);

        item.ApplyBusinessRules();

        Assert.Equal(0.20m, item.DiscountPercentage);
        Assert.Equal(20m, item.DiscountAmount);
        Assert.Equal(80m, item.TotalAmount);
    }

    [Fact(DisplayName = "Item above 20 units should be invalid")]
    public void Given_ItemAboveTwentyUnits_When_ApplyBusinessRules_Then_ShouldThrowDomainException()
    {
        var item = CreateItem(quantity: 21, unitPrice: 10m);

        Assert.Throws<DomainException>(() => item.ApplyBusinessRules());
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
