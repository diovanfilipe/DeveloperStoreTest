using System.ComponentModel.DataAnnotations;

namespace DeveloperStore.SalesApi.Api.Requests;

/// <summary>Dados para atualização completa de uma venda.</summary>
public sealed class UpdateSaleRequest
{
    /// <summary>Data em que a venda foi realizada.</summary>
    [Required]
    public DateTime SaleDate { get; init; }

    /// <summary>Identificador do cliente.</summary>
    [Required]
    public Guid CustomerId { get; init; }

    /// <summary>Nome do cliente.</summary>
    [Required]
    [MaxLength(150)]
    public string CustomerName { get; init; } = string.Empty;

    /// <summary>Identificador da filial.</summary>
    [Required]
    public Guid BranchId { get; init; }

    /// <summary>Nome da filial.</summary>
    [Required]
    [MaxLength(150)]
    public string BranchName { get; init; } = string.Empty;

    /// <summary>Itens da venda.</summary>
    [Required]
    [MinLength(1)]
    public List<UpdateSaleItemRequest> Items { get; init; } = [];
}
