using DeveloperStore.SalesApi.Application.Sales.Dtos;
using MediatR;

namespace DeveloperStore.SalesApi.Application.Sales.Commands.CreateSale;

public sealed record CreateSaleCommand(
    string IdempotencyKey,
    DateTime SaleDate,
    Guid CustomerId,
    string CustomerName,
    Guid BranchId,
    string BranchName,
    List<SaleItemInputDto> Items) : IRequest<SaleDto>;
