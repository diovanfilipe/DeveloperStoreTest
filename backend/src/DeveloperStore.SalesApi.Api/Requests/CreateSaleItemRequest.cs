using System.ComponentModel.DataAnnotations;

namespace DeveloperStore.SalesApi.Api.Requests;

/// <summary>Dados de um item da venda.</summary>
public sealed class CreateSaleItemRequest
{
    /// <summary>Identificador do produto.</summary>
    [Required]
    public Guid ProductId { get; init; }

    /// <summary>Nome do produto.</summary>
    [Required]
    [MaxLength(150)]
    public string ProductName { get; init; } = string.Empty;

    /// <summary>Quantidade do produto na venda.</summary>
    [Range(1, 20)]
    public int Quantity { get; init; }

    /// <summary>Preço unitário do produto.</summary>
    [Range(typeof(decimal), "0.01", "999999999", ConvertValueInInvariantCulture = true, ParseLimitsInInvariantCulture = true)]
    public decimal UnitPrice { get; init; }
}
