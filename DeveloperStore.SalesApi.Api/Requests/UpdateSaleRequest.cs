using System.ComponentModel.DataAnnotations;

namespace DeveloperStore.SalesApi.Api.Requests;

public sealed class UpdateSaleRequest
{
    [Required]
    public DateTime SaleDate { get; init; }

    [Required]
    public Guid CustomerId { get; init; }

    [Required]
    [MaxLength(150)]
    public string CustomerName { get; init; } = string.Empty;

    [Required]
    public Guid BranchId { get; init; }

    [Required]
    [MaxLength(150)]
    public string BranchName { get; init; } = string.Empty;

    [Required]
    [MinLength(1)]
    public List<CreateSaleItemRequest> Items { get; init; } = [];
}
