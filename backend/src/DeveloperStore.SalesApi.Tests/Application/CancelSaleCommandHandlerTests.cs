using DeveloperStore.SalesApi.Application.Abstractions;
using DeveloperStore.SalesApi.Application.Sales.Commands.CancelSale;
using DeveloperStore.SalesApi.Domain.Enums;
using DeveloperStore.SalesApi.Domain.Exceptions;
using DeveloperStore.SalesApi.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace DeveloperStore.SalesApi.Tests.Application;

public sealed class CancelSaleCommandHandlerTests
{
    private readonly Mock<IEventPublisher> _eventPublisherMock = new();
    private readonly Mock<ISaleRepository> _saleRepositoryMock = new();

    [Fact]
    public async Task Handle_ShouldCancelSale_WhenSaleIsActive()
    {
        var sale = TestSaleFactory.CreateActiveSale();
        _saleRepositoryMock.Setup(repository => repository.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sale);

        var handler = new CancelSaleCommandHandler(_saleRepositoryMock.Object, _eventPublisherMock.Object);

        var result = await handler.Handle(new CancelSaleCommand(sale.Id), CancellationToken.None);

        result.Status.Should().Be(SaleStatus.Cancelled);
        result.Items.Should().OnlyContain(item => item.Status == SaleStatus.Cancelled);
        result.TotalSaleAmount.Should().Be(0m);

        _saleRepositoryMock.Verify(repository => repository.UpdateAsync(sale, It.IsAny<CancellationToken>()), Times.Once);
        _eventPublisherMock.Verify(
            publisher => publisher.PublishAsync(
                "SaleCancelled",
                sale.Id,
                It.Is<string>(message => message.Contains(sale.SaleNumber)),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _eventPublisherMock.Verify(
            publisher => publisher.PublishAsync(
                "ItemCancelled",
                sale.Id,
                It.Is<string>(message => message.Contains(sale.SaleNumber)),
                It.IsAny<CancellationToken>()),
            Times.Exactly(sale.Items.Count));
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenSaleIsAlreadyCancelled()
    {
        var sale = TestSaleFactory.CreateCancelledSale();
        _saleRepositoryMock.Setup(repository => repository.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sale);

        var handler = new CancelSaleCommandHandler(_saleRepositoryMock.Object, _eventPublisherMock.Object);

        var act = () => handler.Handle(new CancelSaleCommand(sale.Id), CancellationToken.None);

        await act.Should().ThrowAsync<DomainRuleException>()
            .WithMessage("*cannot be cancelled again*");
    }
}
