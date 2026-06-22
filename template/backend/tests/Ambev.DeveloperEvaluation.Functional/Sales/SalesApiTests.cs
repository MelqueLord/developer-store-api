using System.Net;
using System.Net.Http.Json;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales;

public class SalesApiTests : IClassFixture<SalesApiFactory>
{
    private readonly HttpClient _client;

    public SalesApiTests(SalesApiFactory factory)
    {
        factory.ResetDatabase();
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "Given valid sale payload When posting sale Then API creates sale with discounts")]
    public async Task CreateSale_ValidPayload_ReturnsCreatedSaleWithCalculatedDiscounts()
    {
        var request = CreateSaleRequest(quantity: 10, unitPrice: 100m);

        var response = await _client.PostAsJsonAsync("/api/sales", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();

        Assert.NotNull(body);
        Assert.True(body.Success);
        Assert.NotNull(body.Data);
        Assert.Equal("SALE-HTTP-001", body.Data.SaleNumber);
        Assert.Equal(800m, body.Data.TotalAmount);
        var item = Assert.Single(body.Data.Items);
        Assert.Equal(0.20m, item.DiscountPercentage);
        Assert.Equal(200m, item.DiscountAmount);
        Assert.Equal(800m, item.TotalAmount);
    }

    [Fact(DisplayName = "Given created sale When getting by id Then API returns sale details")]
    public async Task GetSale_CreatedSale_ReturnsSaleDetails()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/sales", CreateSaleRequest());
        var createBody = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        var saleId = createBody!.Data!.Id;

        var response = await _client.GetAsync($"/api/sales/{saleId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponseWithData<GetSaleResponse>>();

        Assert.NotNull(body);
        Assert.True(body.Success);
        Assert.NotNull(body.Data);
        Assert.Equal(saleId, body.Data.Id);
        Assert.Equal("Functional Customer", body.Data.CustomerName);
        Assert.False(body.Data.IsCancelled);
        Assert.Single(body.Data.Items);
    }

    [Fact(DisplayName = "Given invalid sale quantity When posting sale Then API returns validation error")]
    public async Task CreateSale_QuantityAboveLimit_ReturnsBadRequest()
    {
        var request = CreateSaleRequest(quantity: 21);

        var response = await _client.PostAsJsonAsync("/api/sales", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();

        Assert.NotNull(body);
        Assert.False(body.Success);
        Assert.Equal("Validation Failed", body.Message);
        Assert.NotEmpty(body.Errors);
    }

    private static CreateSaleRequest CreateSaleRequest(int quantity = 4, decimal unitPrice = 10m)
    {
        return new CreateSaleRequest
        {
            SaleNumber = "SALE-HTTP-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Functional Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Functional Branch",
            Items =
            [
                new CreateSaleItemRequest
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Functional Product",
                    Quantity = quantity,
                    UnitPrice = unitPrice
                }
            ]
        };
    }
}

public class SalesApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<DefaultContext>>();

            services.AddDbContext<DefaultContext>(options =>
                options.UseInMemoryDatabase("sales-api-tests"));
        });
    }

    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}
