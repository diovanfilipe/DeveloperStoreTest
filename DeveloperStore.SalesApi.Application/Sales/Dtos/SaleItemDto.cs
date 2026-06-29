using DeveloperStore.SalesApi.Domain.Enums;

namespace DeveloperStore.SalesApi.Application.Sales.Dtos;

/// <summary>Representação de um item da venda.</summary>
public sealed record SaleItemDto(
    /// <summary>Identificador do item.</summary>
    Guid Id,
    /// <summary>Identificador do produto.</summary>
    Guid ProductId,
    /// <summary>Nome do produto.</summary>
    string ProductName,
    /// <summary>Quantidade vendida.</summary>
    int Quantity,
    /// <summary>Preço unitário.</summary>
    decimal UnitPrice,
    /// <summary>Percentual de desconto aplicado.</summary>
    decimal DiscountPercent,
    /// <summary>Valor descontado.</summary>
    decimal DiscountValue,
    /// <summary>Valor total do item.</summary>
    decimal TotalItemAmount,
    /// <summary>Status atual do item.</summary>
    SaleStatus Status);
