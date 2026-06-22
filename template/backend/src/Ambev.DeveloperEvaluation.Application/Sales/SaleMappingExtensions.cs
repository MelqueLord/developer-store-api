using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public static class SaleMappingExtensions
{
    public static Sale ToEntity(this CreateSaleCommand command)
    {
        return new Sale
        {
            SaleNumber = command.SaleNumber,
            SaleDate = command.SaleDate,
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            Items = command.Items.Select(item => item.ToEntity()).ToList()
        };
    }

    public static SaleItem ToEntity(this CreateSaleItemCommand command)
    {
        return new SaleItem
        {
            ProductId = command.ProductId,
            ProductName = command.ProductName,
            Quantity = command.Quantity,
            UnitPrice = command.UnitPrice
        };
    }

    public static SaleItem ToEntity(this UpdateSaleItemCommand command)
    {
        return new SaleItem
        {
            ProductId = command.ProductId,
            ProductName = command.ProductName,
            Quantity = command.Quantity,
            UnitPrice = command.UnitPrice
        };
    }

    public static GetSaleResult ToGetSaleResult(this Sale sale)
    {
        return new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            BranchId = sale.BranchId,
            BranchName = sale.BranchName,
            TotalAmount = sale.TotalAmount,
            IsCancelled = sale.IsCancelled,
            Items = sale.Items.Select(item => item.ToGetSaleItemResult()).ToList()
        };
    }

    public static ListSalesResult ToListSalesResult(this Sale sale)
    {
        return new ListSalesResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            BranchId = sale.BranchId,
            BranchName = sale.BranchName,
            TotalAmount = sale.TotalAmount,
            IsCancelled = sale.IsCancelled,
            Items = sale.Items.Select(item => item.ToGetSaleItemResult()).ToList()
        };
    }

    public static CreateSaleResult ToCreateSaleResult(this Sale sale)
    {
        return new CreateSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            BranchId = sale.BranchId,
            BranchName = sale.BranchName,
            TotalAmount = sale.TotalAmount,
            IsCancelled = sale.IsCancelled,
            Items = sale.Items.Select(item => item.ToGetSaleItemResult()).ToList()
        };
    }

    public static UpdateSaleResult ToUpdateSaleResult(this Sale sale)
    {
        return new UpdateSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            BranchId = sale.BranchId,
            BranchName = sale.BranchName,
            TotalAmount = sale.TotalAmount,
            IsCancelled = sale.IsCancelled,
            Items = sale.Items.Select(item => item.ToGetSaleItemResult()).ToList()
        };
    }

    public static GetSaleItemResult ToGetSaleItemResult(this SaleItem item)
    {
        return new GetSaleItemResult
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            DiscountPercentage = item.DiscountPercentage,
            DiscountAmount = item.DiscountAmount,
            TotalAmount = item.TotalAmount,
            IsCancelled = item.IsCancelled
        };
    }
}
