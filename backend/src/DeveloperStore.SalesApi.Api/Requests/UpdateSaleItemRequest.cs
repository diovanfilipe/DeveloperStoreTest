using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DeveloperStore.SalesApi.Api.Requests;

/// <summary>Dados de um item da venda em atualização.</summary>
public sealed class UpdateSaleItemRequest
{
    /// <summary>Identificador do item, quando existente.</summary>
    [JsonPropertyName("id")]
    public Guid? ItemId { get; init; }

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
