using DeveloperStore.SalesApi.Domain.Enums;

namespace DeveloperStore.SalesApi.Application.Sales.Dtos;

public sealed record SaleItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercent,
    decimal DiscountValue,
    decimal TotalItemAmount,
    SaleStatus Status);
