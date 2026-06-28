using DeveloperStore.SalesApi.Application.Abstractions;
using DeveloperStore.SalesApi.Application.Common.Exceptions;
using DeveloperStore.SalesApi.Application.Sales.Dtos;
using DeveloperStore.SalesApi.Application.Sales.Mappings;
using DeveloperStore.SalesApi.Application.Sales.Support;
using DeveloperStore.SalesApi.Domain.Repositories;
using MediatR;

namespace DeveloperStore.SalesApi.Application.Sales.Commands.CancelSaleItem;

public sealed class CancelSaleItemCommandHandler : IRequestHandler<CancelSaleItemCommand, SaleDto>
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ISaleRepository _saleRepository;

    public CancelSaleItemCommandHandler(ISaleRepository saleRepository, IEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<SaleDto> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale is null)
        {
            throw new NotFoundException("Sale was not found.");
        }

        SaleHandlerSupport.CancelItem(sale, request.ItemId);
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        await _eventPublisher.PublishAsync(
            "ItemCancelled",
            sale.Id,
            $"Item {request.ItemId} cancelled from sale {sale.SaleNumber}.",
            cancellationToken);

        return sale.ToDto();
    }
}
