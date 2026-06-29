using DeveloperStore.SalesApi.Domain.Enums;
using DeveloperStore.SalesApi.Domain.Exceptions;

namespace DeveloperStore.SalesApi.Domain.Entities;

public sealed class SaleItem
{
    private SaleItem()
    {
    }

    public Guid Id { get; private set; }

    public Guid ProductId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal DiscountPercent { get; private set; }

    public decimal DiscountValue { get; private set; }

    public decimal TotalItemAmount { get; private set; }

    public SaleStatus Status { get; private set; }

    public static SaleItem Create(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        Validate(productId, productName, quantity, unitPrice);

        var item = new SaleItem
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            ProductName = productName.Trim(),
            Quantity = quantity,
            UnitPrice = unitPrice,
            Status = SaleStatus.Active
        };

        item.Recalculate();
        return item;
    }

    public static SaleItem Restore(
        Guid id,
        Guid productId,
        string productName,
        int quantity,
        decimal unitPrice,
        decimal discountPercent,
        decimal discountValue,
        decimal totalItemAmount,
        SaleStatus status)
    {
        return new SaleItem
        {
            Id = id,
            ProductId = productId,
            ProductName = productName,
            Quantity = quantity,
            UnitPrice = unitPrice,
            DiscountPercent = discountPercent,
            DiscountValue = discountValue,
            TotalItemAmount = totalItemAmount,
            Status = status
        };
    }

    public void Cancel()
    {
        if (Status == SaleStatus.Cancelled)
        {
            throw new DomainRuleException("Cancelled item cannot be cancelled again.");
        }

        Status = SaleStatus.Cancelled;
    }

    private static void Validate(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (productId == Guid.Empty)
        {
            throw new DomainRuleException("ProductId is required.");
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new DomainRuleException("ProductName is required.");
        }

        if (quantity <= 0)
        {
            throw new DomainRuleException("Item quantity must be greater than zero.");
        }

        if (quantity > 20)
        {
            throw new DomainRuleException("It is not possible to sell above 20 identical items.");
        }

        if (unitPrice <= 0)
        {
            throw new DomainRuleException("Unit price must be greater than zero.");
        }
    }

    private void Recalculate()
    {
        DiscountPercent = Quantity switch
        {
            >= 10 and <= 20 => 20m,
            >= 4 => 10m,
            _ => 0m
        };

        var grossAmount = Quantity * UnitPrice;
        DiscountValue = Math.Round(grossAmount * (DiscountPercent / 100m), 2, MidpointRounding.AwayFromZero);
        TotalItemAmount = grossAmount - DiscountValue;
    }
}
