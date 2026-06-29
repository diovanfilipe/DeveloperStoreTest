namespace DeveloperStore.SalesApi.Application.Sales.Dtos;

public sealed record SaleItemInputDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);
