using DeveloperStore.SalesApi.Domain.Entities;

namespace DeveloperStore.SalesApi.Tests.Application;

internal static class TestSaleFactory
{
    public static Sale CreateActiveSale()
    {
        return Sale.Create(
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Customer",
            Guid.NewGuid(),
            "Branch",
            [SaleItem.Create(Guid.NewGuid(), "Product", 4, 10m)]);
    }

    public static Sale CreateCancelledSale()
    {
        var sale = CreateActiveSale();
        sale.Cancel();
        return sale;
    }

    public static Sale CreateActiveSaleWithTwoItems()
    {
        return Sale.Create(
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Customer",
            Guid.NewGuid(),
            "Branch",
            [
                SaleItem.Create(Guid.NewGuid(), "Product A", 4, 10m),
                SaleItem.Create(Guid.NewGuid(), "Product B", 2, 10m)
            ]);
    }
}
