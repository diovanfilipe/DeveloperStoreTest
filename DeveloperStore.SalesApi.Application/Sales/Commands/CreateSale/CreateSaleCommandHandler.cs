using DeveloperStore.SalesApi.Application.Abstractions;
using DeveloperStore.SalesApi.Application.Sales.Dtos;
using DeveloperStore.SalesApi.Application.Sales.Mappings;
using DeveloperStore.SalesApi.Application.Sales.Support;
using DeveloperStore.SalesApi.Domain.Repositories;
using MediatR;

namespace DeveloperStore.SalesApi.Application.Sales.Commands.CreateSale;

public sealed class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, SaleDto>
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ISaleRepository _saleRepository;

    public CreateSaleCommandHandler(ISaleRepository saleRepository, IEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<SaleDto> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = SaleHandlerSupport.BuildNewSale(
            request.SaleDate,
            request.CustomerId,
            request.CustomerName,
            request.BranchId,
            request.BranchName,
            request.Items);

        var persistedSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        await _eventPublisher.PublishAsync(
            "SaleCreated",
            persistedSale.Id,
            $"Sale {persistedSale.SaleNumber} created.",
            cancellationToken);

        return persistedSale.ToDto();
    }
}
