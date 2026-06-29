using DeveloperStore.SalesApi.Application.Abstractions;
using DeveloperStore.SalesApi.Application.Common.Exceptions;
using DeveloperStore.SalesApi.Application.Sales.Commands.UpdateSale;
using DeveloperStore.SalesApi.Application.Sales.Dtos;
using DeveloperStore.SalesApi.Domain.Entities;
using DeveloperStore.SalesApi.Domain.Exceptions;
using DeveloperStore.SalesApi.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace DeveloperStore.SalesApi.Tests.Application;

public sealed class UpdateSaleCommandHandlerTests
{
    private readonly Mock<IEventPublisher> _eventPublisherMock = new();
    private readonly Mock<ISaleRepository> _saleRepositoryMock = new();

    [Fact]
    public async Task Handle_ShouldUpdateSale_WhenSaleIsActive()
    {
        var sale = TestSaleFactory.CreateActiveSale();
        var existingItemId = sale.Items.First().Id;
        var command = new UpdateSaleCommand(
            sale.Id,
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Updated Customer",
            Guid.NewGuid(),
            "Updated Branch",
            [new UpdateSaleItemInputDto(existingItemId, Guid.NewGuid(), "Updated Product", 4, 10m)]);

        _saleRepositoryMock
            .Setup(repository => repository.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var handler = new UpdateSaleCommandHandler(_saleRepositoryMock.Object, _eventPublisherMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.CustomerName.Should().Be("Updated Customer");
        result.TotalSaleAmount.Should().Be(36m);
        _saleRepositoryMock.Verify(repository => repository.UpdateAsync(sale, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenSaleDoesNotExist()
    {
        var command = new UpdateSaleCommand(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch", []);

        _saleRepositoryMock
            .Setup(repository => repository.GetByIdAsync(command.SaleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        var handler = new UpdateSaleCommandHandler(_saleRepositoryMock.Object, _eventPublisherMock.Object);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenSaleIsCancelled()
    {
        var sale = TestSaleFactory.CreateCancelledSale();
        var command = new UpdateSaleCommand(
            sale.Id,
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Customer",
            Guid.NewGuid(),
            "Branch",
            [new UpdateSaleItemInputDto(Guid.NewGuid(), Guid.NewGuid(), "Product", 4, 10m)]);

        _saleRepositoryMock
            .Setup(repository => repository.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var handler = new UpdateSaleCommandHandler(_saleRepositoryMock.Object, _eventPublisherMock.Object);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainRuleException>()
            .WithMessage("*cannot be changed*");
    }
}
