using DeveloperStore.SalesApi.Application.Common.Exceptions;
using DeveloperStore.SalesApi.Application.Sales.Dtos;
using DeveloperStore.SalesApi.Application.Sales.Mappings;
using DeveloperStore.SalesApi.Domain.Repositories;
using MediatR;

namespace DeveloperStore.SalesApi.Application.Sales.Queries.GetSaleById;

public sealed class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleDto>
{
    private readonly ISaleRepository _saleRepository;

    public GetSaleByIdQueryHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<SaleDto> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale is null)
        {
            throw new NotFoundException("Sale was not found.");
        }

        return sale.ToDto();
    }
}
