using DeveloperStore.SalesApi.Domain.Entities;
using DeveloperStore.SalesApi.Domain.Enums;
using DeveloperStore.SalesApi.Domain.Exceptions;
using FluentAssertions;

namespace DeveloperStore.SalesApi.Tests.Domain;

public sealed class SaleTests
{
    [Fact]
    public void Cancel_ShouldCancelAllActiveItemsAndReturnOnlyCancelledItems()
    {
        var sale = CreateSale();

        var cancelledItems = sale.Cancel();

        cancelledItems.Should().HaveCount(2);
        cancelledItems.Should().OnlyContain(item => item.Status == SaleStatus.Cancelled);
        sale.Items.Should().OnlyContain(item => item.Status == SaleStatus.Cancelled);
        sale.Status.Should().Be(SaleStatus.Cancelled);
        sale.TotalSaleAmount.Should().Be(0m);
    }

    [Fact]
    public void Cancel_ShouldKeepPreviouslyCancelledItemsAndNotReturnThem()
    {
        var sale = CreateSale();
        var firstItemId = sale.Items.First().Id;
        var secondItemId = sale.Items.Last().Id;

        sale.CancelItem(firstItemId);

        var cancelledItems = sale.Cancel();

        cancelledItems.Should().HaveCount(1);
        cancelledItems.Single().Id.Should().Be(secondItemId);
        sale.Items.First(item => item.Id == firstItemId).Status.Should().Be(SaleStatus.Cancelled);
        sale.Items.First(item => item.Id == secondItemId).Status.Should().Be(SaleStatus.Cancelled);
        sale.TotalSaleAmount.Should().Be(0m);
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenSaleIsAlreadyCancelled()
    {
        var sale = CreateSale();

        sale.Cancel();
        var act = () => sale.Cancel();

        act.Should().Throw<DomainRuleException>()
            .WithMessage("*cannot be cancelled again*");
    }

    private static Sale CreateSale()
    {
        return Sale.Create(
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Customer",
            Guid.NewGuid(),
            "Branch",
            [
                SaleItem.Create(Guid.NewGuid(), "Product A", 4, 10m),
                SaleItem.Create(Guid.NewGuid(), "Product B", 2, 10m)
            ]);
    }
}
