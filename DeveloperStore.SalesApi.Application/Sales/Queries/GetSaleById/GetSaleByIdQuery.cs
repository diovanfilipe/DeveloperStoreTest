using DeveloperStore.SalesApi.Application.Sales.Dtos;
using MediatR;

namespace DeveloperStore.SalesApi.Application.Sales.Queries.GetSaleById;

public sealed record GetSaleByIdQuery(Guid SaleId) : IRequest<SaleDto>;
