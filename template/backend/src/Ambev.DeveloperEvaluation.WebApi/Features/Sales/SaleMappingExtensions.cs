using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public static class SaleMappingExtensions
{
    public static CreateSaleCommand ToCommand(this CreateSaleRequest request)
    {
        return new CreateSaleCommand
        {
            SaleNumber = request.SaleNumber,
            SaleDate = request.SaleDate,
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            BranchId = request.BranchId,
            BranchName = request.BranchName,
            Items = request.Items.Select(item => item.ToCommand()).ToList()
        };
    }

    public static CreateSaleItemCommand ToCommand(this CreateSaleItemRequest request)
    {
        return new CreateSaleItemCommand
        {
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice
        };
    }

    public static UpdateSaleCommand ToCommand(this UpdateSaleRequest request, Guid id)
    {
        return new UpdateSaleCommand
        {
            Id = id,
            SaleNumber = request.SaleNumber,
            SaleDate = request.SaleDate,
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            BranchId = request.BranchId,
            BranchName = request.BranchName,
            Items = request.Items.Select(item => item.ToCommand()).ToList()
        };
    }

    public static UpdateSaleItemCommand ToCommand(this UpdateSaleItemRequest request)
    {
        return new UpdateSaleItemCommand
        {
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice
        };
    }

    public static GetSaleCommand ToGetSaleCommand(this Guid id)
    {
        return new GetSaleCommand(id);
    }

    public static CancelSaleCommand ToCancelSaleCommand(this Guid id)
    {
        return new CancelSaleCommand(id);
    }

    public static GetSaleResponse ToResponse(this GetSaleResult result)
    {
        return new GetSaleResponse
        {
            Id = result.Id,
            SaleNumber = result.SaleNumber,
            SaleDate = result.SaleDate,
            CustomerId = result.CustomerId,
            CustomerName = result.CustomerName,
            BranchId = result.BranchId,
            BranchName = result.BranchName,
            TotalAmount = result.TotalAmount,
            IsCancelled = result.IsCancelled,
            Items = result.Items.Select(item => item.ToResponse()).ToList()
        };
    }

    public static CreateSaleResponse ToResponse(this CreateSaleResult result)
    {
        return new CreateSaleResponse
        {
            Id = result.Id,
            SaleNumber = result.SaleNumber,
            SaleDate = result.SaleDate,
            CustomerId = result.CustomerId,
            CustomerName = result.CustomerName,
            BranchId = result.BranchId,
            BranchName = result.BranchName,
            TotalAmount = result.TotalAmount,
            IsCancelled = result.IsCancelled,
            Items = result.Items.Select(item => item.ToResponse()).ToList()
        };
    }

    public static UpdateSaleResponse ToResponse(this UpdateSaleResult result)
    {
        return new UpdateSaleResponse
        {
            Id = result.Id,
            SaleNumber = result.SaleNumber,
            SaleDate = result.SaleDate,
            CustomerId = result.CustomerId,
            CustomerName = result.CustomerName,
            BranchId = result.BranchId,
            BranchName = result.BranchName,
            TotalAmount = result.TotalAmount,
            IsCancelled = result.IsCancelled,
            Items = result.Items.Select(item => item.ToResponse()).ToList()
        };
    }

    public static ListSalesResponse ToResponse(this ListSalesResult result)
    {
        return new ListSalesResponse
        {
            Id = result.Id,
            SaleNumber = result.SaleNumber,
            SaleDate = result.SaleDate,
            CustomerId = result.CustomerId,
            CustomerName = result.CustomerName,
            BranchId = result.BranchId,
            BranchName = result.BranchName,
            TotalAmount = result.TotalAmount,
            IsCancelled = result.IsCancelled,
            Items = result.Items.Select(item => item.ToResponse()).ToList()
        };
    }

    public static GetSaleItemResponse ToResponse(this GetSaleItemResult result)
    {
        return new GetSaleItemResponse
        {
            Id = result.Id,
            ProductId = result.ProductId,
            ProductName = result.ProductName,
            Quantity = result.Quantity,
            UnitPrice = result.UnitPrice,
            DiscountPercentage = result.DiscountPercentage,
            DiscountAmount = result.DiscountAmount,
            TotalAmount = result.TotalAmount,
            IsCancelled = result.IsCancelled
        };
    }
}
