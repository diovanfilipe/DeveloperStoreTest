using DeveloperStore.SalesApi.Domain.Entities;
using DeveloperStore.SalesApi.Domain.Enums;

namespace DeveloperStore.SalesApi.Tests.Application;

internal static class TestSaleFactory
{
    public static Sale CreateActiveSale()
    {
        return new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = Guid.NewGuid().ToString("N"),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Branch",
            Status = SaleStatus.Active,
            TotalSaleAmount = 36m,
            Items =
            [
                new SaleItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product",
                    Quantity = 4,
                    UnitPrice = 10m,
                    DiscountPercent = 10m,
                    DiscountValue = 4m,
                    TotalItemAmount = 36m,
                    Status = SaleStatus.Active
                }
            ]
        };
    }

    public static Sale CreateCancelledSale()
    {
        var sale = CreateActiveSale();
        sale.Status = SaleStatus.Cancelled;
        return sale;
    }

    public static Sale CreateActiveSaleWithTwoItems()
    {
        return new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = Guid.NewGuid().ToString("N"),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Branch",
            Status = SaleStatus.Active,
            TotalSaleAmount = 56m,
            Items =
            [
                new SaleItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product A",
                    Quantity = 4,
                    UnitPrice = 10m,
                    DiscountPercent = 10m,
                    DiscountValue = 4m,
                    TotalItemAmount = 36m,
                    Status = SaleStatus.Active
                },
                new SaleItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product B",
                    Quantity = 2,
                    UnitPrice = 10m,
                    DiscountPercent = 0m,
                    DiscountValue = 0m,
                    TotalItemAmount = 20m,
                    Status = SaleStatus.Active
                }
            ]
        };
    }
}
