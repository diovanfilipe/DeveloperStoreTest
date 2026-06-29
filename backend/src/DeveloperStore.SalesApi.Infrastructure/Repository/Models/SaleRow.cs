namespace DeveloperStore.SalesApi.Infrastructure.Repository.Models;

public sealed class SaleRow
{
    public string Id { get; set; } = string.Empty;

    public string SaleNumber { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }

    public string CustomerId { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string BranchId { get; set; } = string.Empty;

    public string BranchName { get; set; } = string.Empty;

    public int Status { get; set; }

    public decimal TotalSaleAmount { get; set; }
}
