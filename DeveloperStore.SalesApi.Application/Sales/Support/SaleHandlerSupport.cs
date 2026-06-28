using DeveloperStore.SalesApi.Application.Common.Exceptions;
using DeveloperStore.SalesApi.Application.Sales.Dtos;
using DeveloperStore.SalesApi.Domain.Entities;
using DeveloperStore.SalesApi.Domain.Enums;

namespace DeveloperStore.SalesApi.Application.Sales.Support;

internal static class SaleHandlerSupport
{
    public static Sale BuildNewSale(
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        List<SaleItemInputDto> items)
    {
        ValidateSaleData(saleDate, customerId, customerName, branchId, branchName, items);

        var saleId = Guid.NewGuid();
        var saleItems = BuildSaleItems(items);

        return new Sale
        {
            Id = saleId,
            SaleNumber = saleId.ToString("N"),
            SaleDate = saleDate,
            CustomerId = customerId,
            CustomerName = customerName.Trim(),
            BranchId = branchId,
            BranchName = branchName.Trim(),
            Status = SaleStatus.Active,
            Items = saleItems,
            TotalSaleAmount = CalculateTotalSaleAmount(saleItems)
        };
    }

    public static Sale ApplyUpdate(
        Sale sale,
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        List<SaleItemInputDto> items)
    {
        EnsureSaleCanChange(sale);
        ValidateSaleData(saleDate, customerId, customerName, branchId, branchName, items);

        var saleItems = BuildSaleItems(items);

        sale.SaleDate = saleDate;
        sale.CustomerId = customerId;
        sale.CustomerName = customerName.Trim();
        sale.BranchId = branchId;
        sale.BranchName = branchName.Trim();
        sale.Items = saleItems;
        sale.TotalSaleAmount = CalculateTotalSaleAmount(saleItems);

        return sale;
    }

    public static Sale CancelSale(Sale sale)
    {
        if (sale.Status == SaleStatus.Cancelled)
        {
            throw new BusinessRuleException("Cancelled sale cannot be cancelled again.");
        }

        sale.Status = SaleStatus.Cancelled;
        return sale;
    }

    public static Sale CancelItem(Sale sale, Guid itemId)
    {
        EnsureSaleCanChange(sale);

        var item = sale.Items.SingleOrDefault(currentItem => currentItem.Id == itemId);
        if (item is null)
        {
            throw new BusinessRuleException("Sale item was not found.");
        }

        if (item.Status == SaleStatus.Cancelled)
        {
            throw new BusinessRuleException("Cancelled item cannot be cancelled again.");
        }

        item.Status = SaleStatus.Cancelled;
        sale.TotalSaleAmount = CalculateTotalSaleAmount(sale.Items);

        return sale;
    }

    private static void EnsureSaleCanChange(Sale sale)
    {
        if (sale.Status == SaleStatus.Cancelled)
        {
            throw new BusinessRuleException("Cancelled sale cannot be changed.");
        }
    }

    private static void ValidateSaleData(
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        List<SaleItemInputDto> items)
    {
        if (saleDate == default)
        {
            throw new BusinessRuleException("Sale date is required.");
        }

        if (customerId == Guid.Empty)
        {
            throw new BusinessRuleException("CustomerId is required.");
        }

        if (branchId == Guid.Empty)
        {
            throw new BusinessRuleException("BranchId is required.");
        }

        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new BusinessRuleException("CustomerName is required.");
        }

        if (string.IsNullOrWhiteSpace(branchName))
        {
            throw new BusinessRuleException("BranchName is required.");
        }

        if (items.Count == 0)
        {
            throw new BusinessRuleException("Sale must contain at least one item.");
        }
    }

    private static List<SaleItem> BuildSaleItems(List<SaleItemInputDto> items)
    {
        return items.Select(BuildSaleItem).ToList();
    }

    private static SaleItem BuildSaleItem(SaleItemInputDto item)
    {
        if (item.ProductId == Guid.Empty)
        {
            throw new BusinessRuleException("ProductId is required.");
        }

        if (string.IsNullOrWhiteSpace(item.ProductName))
        {
            throw new BusinessRuleException("ProductName is required.");
        }

        if (item.Quantity <= 0)
        {
            throw new BusinessRuleException("Item quantity must be greater than zero.");
        }

        if (item.Quantity > 20)
        {
            throw new BusinessRuleException("It is not possible to sell above 20 identical items.");
        }

        if (item.UnitPrice <= 0)
        {
            throw new BusinessRuleException("Unit price must be greater than zero.");
        }

        var discountPercent = item.Quantity switch
        {
            >= 10 and <= 20 => 20m,
            >= 4 => 10m,
            _ => 0m
        };

        var grossAmount = item.Quantity * item.UnitPrice;
        var discountValue = Math.Round(grossAmount * (discountPercent / 100m), 2, MidpointRounding.AwayFromZero);

        return new SaleItem
        {
            Id = Guid.NewGuid(),
            ProductId = item.ProductId,
            ProductName = item.ProductName.Trim(),
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            DiscountPercent = discountPercent,
            DiscountValue = discountValue,
            TotalItemAmount = grossAmount - discountValue,
            Status = SaleStatus.Active
        };
    }

    private static decimal CalculateTotalSaleAmount(List<SaleItem> items)
    {
        return items
            .Where(item => item.Status == SaleStatus.Active)
            .Sum(item => item.TotalItemAmount);
    }
}
