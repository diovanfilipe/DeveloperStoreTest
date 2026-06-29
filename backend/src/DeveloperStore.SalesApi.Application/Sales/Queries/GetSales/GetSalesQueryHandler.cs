using DeveloperStore.SalesApi.Application.Sales.Dtos;
using DeveloperStore.SalesApi.Application.Sales.Mappings;
using DeveloperStore.SalesApi.Domain.Repositories;
using MediatR;

namespace DeveloperStore.SalesApi.Application.Sales.Queries.GetSales;

public sealed class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, List<SaleDto>>
{
    private readonly ISaleRepository _saleRepository;

    public GetSalesQueryHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<List<SaleDto>> Handle(GetSalesQuery request, CancellationToken cancellationToken)
    {
        var sales = await _saleRepository.GetAllAsync(cancellationToken);
        return sales.Select(sale => sale.ToDto()).ToList();
    }
}
