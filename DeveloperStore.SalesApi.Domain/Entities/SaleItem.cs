namespace DeveloperStore.SalesApi.Domain.Entities;

public sealed class SaleItem
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal DiscountValue { get; set; }

    public decimal TotalItemAmount { get; set; }

    public Enums.SaleStatus Status { get; set; }
}
