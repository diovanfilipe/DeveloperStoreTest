using DeveloperStore.SalesApi.Application.Sales.Dtos;
using DeveloperStore.SalesApi.Domain.Entities;

namespace DeveloperStore.SalesApi.Application.Sales.Mappings;

public static class SaleMappings
{
    public static SaleDto ToDto(this Sale sale)
    {
        return new SaleDto(
            sale.Id,
            sale.SaleNumber,
            sale.SaleDate,
            sale.CustomerId,
            sale.CustomerName,
            sale.BranchId,
            sale.BranchName,
            sale.Status,
            sale.TotalSaleAmount,
            sale.Items.Select(item => new SaleItemDto(
                item.Id,
                item.ProductId,
                item.ProductName,
                item.Quantity,
                item.UnitPrice,
                item.DiscountPercent,
                item.DiscountValue,
                item.TotalItemAmount,
                item.Status)).ToList());
    }
}
