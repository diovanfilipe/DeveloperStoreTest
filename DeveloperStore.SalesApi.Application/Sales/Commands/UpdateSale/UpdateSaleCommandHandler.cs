using DeveloperStore.SalesApi.Application.Abstractions;
using DeveloperStore.SalesApi.Application.Common.Exceptions;
using DeveloperStore.SalesApi.Application.Sales.Dtos;
using DeveloperStore.SalesApi.Application.Sales.Mappings;
using DeveloperStore.SalesApi.Domain.Entities;
using DeveloperStore.SalesApi.Domain.Repositories;
using MediatR;

namespace DeveloperStore.SalesApi.Application.Sales.Commands.UpdateSale;

public sealed class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, SaleDto>
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ISaleRepository _saleRepository;

    public UpdateSaleCommandHandler(ISaleRepository saleRepository, IEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<SaleDto> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var existingSale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (existingSale is null)
        {
            throw new NotFoundException("Sale was not found.");
        }

        existingSale.Update(
            request.SaleDate,
            request.CustomerId,
            request.CustomerName,
            request.BranchId,
            request.BranchName,
            request.Items.Select(item => SaleItem.Create(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice)).ToList());

        await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        await _eventPublisher.PublishAsync(
            "SaleModified",
            existingSale.Id,
            $"Sale {existingSale.SaleNumber} updated.",
            cancellationToken);

        return existingSale.ToDto();
    }
}
