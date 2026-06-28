using DeveloperStore.SalesApi.Application.Sales.Dtos;
using MediatR;

namespace DeveloperStore.SalesApi.Application.Sales.Commands.UpdateSale;

public sealed record UpdateSaleCommand(
    Guid SaleId,
    DateTime SaleDate,
    Guid CustomerId,
    string CustomerName,
    Guid BranchId,
    string BranchName,
    List<SaleItemInputDto> Items) : IRequest<SaleDto>;
