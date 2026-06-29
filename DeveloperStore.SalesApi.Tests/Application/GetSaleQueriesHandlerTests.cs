using DeveloperStore.SalesApi.Application.Common.Exceptions;
using DeveloperStore.SalesApi.Application.Sales.Queries.GetSaleById;
using DeveloperStore.SalesApi.Application.Sales.Queries.GetSales;
using DeveloperStore.SalesApi.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace DeveloperStore.SalesApi.Tests.Application;

public sealed class GetSaleQueriesHandlerTests
{
    [Fact]
    public async Task GetById_ShouldReturnSale()
    {
        var sale = TestSaleFactory.CreateActiveSale();
        var repositoryMock = new Mock<ISaleRepository>();
        repositoryMock.Setup(repository => repository.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sale);

        var handler = new GetSaleByIdQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetSaleByIdQuery(sale.Id), CancellationToken.None);

        result.Id.Should().Be(sale.Id);
    }

    [Fact]
    public async Task GetById_ShouldThrow_WhenSaleDoesNotExist()
    {
        var repositoryMock = new Mock<ISaleRepository>();
        repositoryMock.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((DeveloperStore.SalesApi.Domain.Entities.Sale?)null);

        var handler = new GetSaleByIdQueryHandler(repositoryMock.Object);

        var act = () => handler.Handle(new GetSaleByIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetSales_ShouldReturnMappedSales()
    {
        var repositoryMock = new Mock<ISaleRepository>();
        repositoryMock.Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([TestSaleFactory.CreateActiveSale()]);

        var handler = new GetSalesQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetSalesQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
    }
}
