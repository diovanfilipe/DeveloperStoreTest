using DeveloperStore.SalesApi.Application.Abstractions;
using DeveloperStore.SalesApi.Application.Common.Exceptions;
using DeveloperStore.SalesApi.Application.Sales.Dtos;
using DeveloperStore.SalesApi.Application.Sales.Mappings;
using DeveloperStore.SalesApi.Application.Sales.Support;
using DeveloperStore.SalesApi.Domain.Repositories;
using MediatR;

namespace DeveloperStore.SalesApi.Application.Sales.Commands.CancelSale;

public sealed class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, SaleDto>
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ISaleRepository _saleRepository;

    public CancelSaleCommandHandler(ISaleRepository saleRepository, IEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<SaleDto> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale is null)
        {
            throw new NotFoundException("Sale was not found.");
        }

        SaleHandlerSupport.CancelSale(sale);
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        await _eventPublisher.PublishAsync(
            "SaleCancelled",
            sale.Id,
            $"Sale {sale.SaleNumber} cancelled.",
            cancellationToken);

        return sale.ToDto();
    }
}
