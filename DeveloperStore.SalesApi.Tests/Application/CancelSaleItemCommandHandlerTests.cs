using DeveloperStore.SalesApi.Application.Abstractions;
using DeveloperStore.SalesApi.Application.Sales.Commands.CancelSaleItem;
using DeveloperStore.SalesApi.Domain.Exceptions;
using DeveloperStore.SalesApi.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace DeveloperStore.SalesApi.Tests.Application;

public sealed class CancelSaleItemCommandHandlerTests
{
    private readonly Mock<IEventPublisher> _eventPublisherMock = new();
    private readonly Mock<ISaleRepository> _saleRepositoryMock = new();

    [Fact]
    public async Task Handle_ShouldCancelItemAndRecalculateTotal()
    {
        var sale = TestSaleFactory.CreateActiveSaleWithTwoItems();
        var itemId = sale.Items.First().Id;

        _saleRepositoryMock.Setup(repository => repository.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sale);

        var handler = new CancelSaleItemCommandHandler(_saleRepositoryMock.Object, _eventPublisherMock.Object);

        var result = await handler.Handle(new CancelSaleItemCommand(sale.Id, itemId), CancellationToken.None);

        result.Items.First(item => item.Id == itemId).Status.Should().Be(Domain.Enums.SaleStatus.Cancelled);
        result.TotalSaleAmount.Should().Be(20m);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenItemIsAlreadyCancelled()
    {
        var sale = TestSaleFactory.CreateActiveSale();
        sale.CancelItem(sale.Items[0].Id);

        _saleRepositoryMock.Setup(repository => repository.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sale);

        var handler = new CancelSaleItemCommandHandler(_saleRepositoryMock.Object, _eventPublisherMock.Object);

        var act = () => handler.Handle(new CancelSaleItemCommand(sale.Id, sale.Items[0].Id), CancellationToken.None);

        await act.Should().ThrowAsync<DomainRuleException>()
            .WithMessage("*cannot be cancelled again*");
    }
}
