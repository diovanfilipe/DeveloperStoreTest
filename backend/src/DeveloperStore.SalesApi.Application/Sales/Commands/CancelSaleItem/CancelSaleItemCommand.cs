using DeveloperStore.SalesApi.Application.Sales.Dtos;
using MediatR;

namespace DeveloperStore.SalesApi.Application.Sales.Commands.CancelSaleItem;

public sealed record CancelSaleItemCommand(Guid SaleId, Guid ItemId) : IRequest<SaleDto>;
