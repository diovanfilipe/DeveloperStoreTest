using DeveloperStore.SalesApi.Domain.Enums;

namespace DeveloperStore.SalesApi.Application.Sales.Dtos;

public sealed record SaleDto(
    Guid Id,
    string SaleNumber,
    DateTime SaleDate,
    Guid CustomerId,
    string CustomerName,
    Guid BranchId,
    string BranchName,
    SaleStatus Status,
    decimal TotalSaleAmount,
    List<SaleItemDto> Items);
