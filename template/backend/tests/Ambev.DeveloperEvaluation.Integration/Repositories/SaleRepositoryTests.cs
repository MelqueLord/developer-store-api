using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Repositories;

public class SaleRepositoryTests
{
    [Fact(DisplayName = "Given a sale When saved Then repository persists sale with calculated totals")]
    public async Task CreateAsync_ValidSale_PersistsSaleWithItemsAndTotals()
    {
        await using var context = CreateContext();
        var repository = new SaleRepository(context);
        var sale = CreateSale("SALE-001", quantity: 10, unitPrice: 50m);

        await repository.CreateAsync(sale, CancellationToken.None);

        var storedSale = await repository.GetByIdAsync(sale.Id, CancellationToken.None);

        Assert.NotNull(storedSale);
        Assert.Equal("SALE-001", storedSale.SaleNumber);
        Assert.Equal(400m, storedSale.TotalAmount);
        var storedItem = Assert.Single(storedSale.Items);
        Assert.Equal(0.20m, storedItem.DiscountPercentage);
        Assert.Equal(100m, storedItem.DiscountAmount);
        Assert.Equal(400m, storedItem.TotalAmount);
    }

    [Fact(DisplayName = "Given saved sales When listing with filters Then repository returns matching page")]
    public async Task GetPagedAsync_WithFilters_ReturnsMatchingSales()
    {
        await using var context = CreateContext();
        var repository = new SaleRepository(context);

        await repository.CreateAsync(CreateSale("SALE-001", "Customer A", "Branch A"), CancellationToken.None);
        await repository.CreateAsync(CreateSale("SALE-002", "Customer B", "Branch B"), CancellationToken.None);

        var result = await repository.GetPagedAsync(
            page: 1,
            size: 10,
            order: "saleNumber asc",
            customerName: "Customer B",
            cancellationToken: CancellationToken.None);

        Assert.Equal(1, result.TotalCount);
        var sale = Assert.Single(result.Items);
        Assert.Equal("SALE-002", sale.SaleNumber);
        Assert.Equal("Customer B", sale.CustomerName);
    }

    [Fact(DisplayName = "Given saved sale When item is cancelled Then repository persists recalculated total")]
    public async Task UpdateAsync_CancelledItem_PersistsCancellationAndRecalculatedTotal()
    {
        await using var context = CreateContext();
        var repository = new SaleRepository(context);
        var sale = CreateSale("SALE-003", quantity: 4, unitPrice: 25m);

        await repository.CreateAsync(sale, CancellationToken.None);
        var itemId = sale.Items.Single().Id;

        sale.CancelItem(itemId);
        await repository.UpdateAsync(sale, CancellationToken.None);

        var storedSale = await repository.GetByIdAsync(sale.Id, CancellationToken.None);

        Assert.NotNull(storedSale);
        Assert.Equal(0m, storedSale.TotalAmount);
        Assert.True(storedSale.Items.Single().IsCancelled);
    }

    private static DefaultContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DefaultContext(options);
    }

    private static Sale CreateSale(
        string saleNumber,
        string customerName = "Customer A",
        string branchName = "Branch A",
        int quantity = 4,
        decimal unitPrice = 10m)
    {
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = saleNumber,
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = customerName,
            BranchId = Guid.NewGuid(),
            BranchName = branchName,
            Items =
            [
                new SaleItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test product",
                    Quantity = quantity,
                    UnitPrice = unitPrice
                }
            ]
        };

        sale.Recalculate();
        return sale;
    }
}
