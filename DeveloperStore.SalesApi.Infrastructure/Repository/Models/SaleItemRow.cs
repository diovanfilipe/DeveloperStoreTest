namespace DeveloperStore.SalesApi.Infrastructure.Repository.Models;

public sealed class SaleItemRow
{
    public string Id { get; set; } = string.Empty;

    public string SaleId { get; set; } = string.Empty;

    public string ProductId { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal DiscountValue { get; set; }

    public decimal TotalItemAmount { get; set; }

    public int Status { get; set; }
}
