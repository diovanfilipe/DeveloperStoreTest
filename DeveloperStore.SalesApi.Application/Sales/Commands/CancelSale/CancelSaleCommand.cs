using DeveloperStore.SalesApi.Application.Sales.Dtos;
using MediatR;

namespace DeveloperStore.SalesApi.Application.Sales.Commands.CancelSale;

public sealed record CancelSaleCommand(Guid SaleId) : IRequest<SaleDto>;
