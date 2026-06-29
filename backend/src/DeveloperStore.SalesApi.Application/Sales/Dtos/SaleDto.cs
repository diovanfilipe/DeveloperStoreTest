using DeveloperStore.SalesApi.Domain.Enums;

namespace DeveloperStore.SalesApi.Application.Sales.Dtos;

/// <summary>Representação de uma venda na API.</summary>
public sealed record SaleDto(
    /// <summary>Identificador da venda.</summary>
    Guid Id,
    /// <summary>Número da venda.</summary>
    string SaleNumber,
    /// <summary>Data da venda.</summary>
    DateTime SaleDate,
    /// <summary>Identificador do cliente.</summary>
    Guid CustomerId,
    /// <summary>Nome do cliente.</summary>
    string CustomerName,
    /// <summary>Identificador da filial.</summary>
    Guid BranchId,
    /// <summary>Nome da filial.</summary>
    string BranchName,
    /// <summary>Status atual da venda.</summary>
    SaleStatus Status,
    /// <summary>Valor total da venda.</summary>
    decimal TotalSaleAmount,
    /// <summary>Itens da venda.</summary>
    List<SaleItemDto> Items);
