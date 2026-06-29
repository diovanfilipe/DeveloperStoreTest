namespace DeveloperStore.SalesApi.Application.Sales.Dtos;

public sealed record UpdateSaleItemInputDto(
    Guid? ItemId,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);
