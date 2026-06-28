using System.ComponentModel.DataAnnotations;

namespace DeveloperStore.SalesApi.Api.Requests;

public sealed class CreateSaleItemRequest
{
    [Required]
    public Guid ProductId { get; init; }

    [Required]
    [MaxLength(150)]
    public string ProductName { get; init; } = string.Empty;

    [Range(1, 20)]
    public int Quantity { get; init; }

    [Range(typeof(decimal), "0.01", "999999999", ConvertValueInInvariantCulture = true, ParseLimitsInInvariantCulture = true)]
    public decimal UnitPrice { get; init; }
}
