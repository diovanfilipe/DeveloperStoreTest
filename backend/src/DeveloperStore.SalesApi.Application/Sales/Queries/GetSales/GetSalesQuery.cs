using DeveloperStore.SalesApi.Application.Sales.Dtos;
using MediatR;

namespace DeveloperStore.SalesApi.Application.Sales.Queries.GetSales;

public sealed record GetSalesQuery() : IRequest<List<SaleDto>>;
