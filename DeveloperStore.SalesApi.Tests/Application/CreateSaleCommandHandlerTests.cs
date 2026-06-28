using DeveloperStore.SalesApi.Application.Abstractions;
using DeveloperStore.SalesApi.Application.Common.Exceptions;
using DeveloperStore.SalesApi.Application.Sales.Commands.CreateSale;
using DeveloperStore.SalesApi.Application.Sales.Dtos;
using DeveloperStore.SalesApi.Domain.Entities;
using DeveloperStore.SalesApi.Domain.Enums;
using DeveloperStore.SalesApi.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace DeveloperStore.SalesApi.Tests.Application;

public sealed class CreateSaleCommandHandlerTests
{
    private readonly Mock<IEventPublisher> _eventPublisherMock = new();
    private readonly Mock<ISaleRepository> _saleRepositoryMock = new();

    [Theory]
    [InlineData(3, 10, 0, 0, 30)]
    [InlineData(4, 10, 10, 4, 36)]
    [InlineData(10, 10, 20, 20, 80)]
    public async Task Handle_ShouldCalculateDiscountsCorrectly(int quantity, decimal unitPrice, decimal expectedPercent, decimal expectedDiscountValue, decimal expectedTotalItemAmount)
    {
        var command = CreateCommand(quantity, unitPrice);

        _saleRepositoryMock
            .Setup(repository => repository.CreateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale sale, CancellationToken _) => sale);

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        var item = result.Items.Single();
        item.DiscountPercent.Should().Be(expectedPercent);
        item.DiscountValue.Should().Be(expectedDiscountValue);
        item.TotalItemAmount.Should().Be(expectedTotalItemAmount);
        result.TotalSaleAmount.Should().Be(expectedTotalItemAmount);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenQuantityIsAboveTwenty()
    {
        var command = CreateCommand(quantity: 21, unitPrice: 10m);

        var handler = CreateHandler();

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*above 20 identical items*");
    }

    private CreateSaleCommandHandler CreateHandler()
    {
        return new CreateSaleCommandHandler(_saleRepositoryMock.Object, _eventPublisherMock.Object);
    }

    private static CreateSaleCommand CreateCommand(int quantity, decimal unitPrice)
    {
        return new CreateSaleCommand(
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Customer",
            Guid.NewGuid(),
            "Branch",
            [new SaleItemInputDto(Guid.NewGuid(), "Product", quantity, unitPrice)]);
    }
}
