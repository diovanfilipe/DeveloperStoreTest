using DeveloperStore.SalesApi.Domain.Entities;
using DeveloperStore.SalesApi.Domain.Enums;
using DeveloperStore.SalesApi.Domain.Exceptions;
using FluentAssertions;

namespace DeveloperStore.SalesApi.Tests.Domain;

public sealed class SaleUpdateTests
{
    [Fact]
    public void Update_ShouldPreserveItemId_WhenItemAlreadyExists()
    {
        var sale = CreateSale();
        var existingItemId = sale.Items.First().Id;

        sale.Update(
            DateTime.UtcNow,
            sale.CustomerId,
            "Updated Customer",
            sale.BranchId,
            "Updated Branch",
            [
                (existingItemId, Guid.NewGuid(), "Updated Product", 4, 10m),
                (null, Guid.NewGuid(), "New Product", 2, 10m)
            ]);

        sale.Items.First(item => item.ProductName == "Updated Product").Id.Should().Be(existingItemId);
    }

    [Fact]
    public void Update_ShouldRecalculateDiscountAndTotal_WhenQuantityOrUnitPriceChanges()
    {
        var sale = CreateSale();
        var existingItemId = sale.Items.First().Id;

        sale.Update(
            DateTime.UtcNow,
            sale.CustomerId,
            "Updated Customer",
            sale.BranchId,
            "Updated Branch",
            [(existingItemId, Guid.NewGuid(), "Updated Product", 10, 20m)]);

        var updatedItem = sale.Items.Single(item => item.Id == existingItemId);
        updatedItem.DiscountPercent.Should().Be(20m);
        updatedItem.DiscountValue.Should().Be(40m);
        updatedItem.TotalItemAmount.Should().Be(160m);
        sale.TotalSaleAmount.Should().Be(160m);
    }

    [Fact]
    public void Update_ShouldAddNewItem_WhenItemIdIsMissing()
    {
        var sale = CreateSale();
        var originalIds = sale.Items.Select(item => item.Id).ToHashSet();

        sale.Update(
            DateTime.UtcNow,
            sale.CustomerId,
            "Updated Customer",
            sale.BranchId,
            "Updated Branch",
            [
                (sale.Items.First().Id, Guid.NewGuid(), "Updated Product", 4, 10m),
                (null, Guid.NewGuid(), "New Product", 2, 10m)
            ]);

        sale.Items.Should().HaveCount(2);
        sale.Items.Count(item => !originalIds.Contains(item.Id)).Should().Be(1);
    }

    [Fact]
    public void Update_ShouldRemoveMissingItem_FromSale()
    {
        var sale = CreateSaleWithTwoItems();
        var firstItemId = sale.Items.First().Id;

        sale.Update(
            DateTime.UtcNow,
            sale.CustomerId,
            "Updated Customer",
            sale.BranchId,
            "Updated Branch",
            [(firstItemId, Guid.NewGuid(), "Updated Product", 4, 10m)]);

        sale.Items.Should().HaveCount(1);
        sale.Items.Single().Id.Should().Be(firstItemId);
    }

    [Fact]
    public void Update_ShouldPreserveCancelledItemStatus_WhenItemRemainsInPayload()
    {
        var sale = CreateSaleWithTwoItems();
        var cancelledItem = sale.Items.First();
        sale.CancelItem(cancelledItem.Id);

        sale.Update(
            DateTime.UtcNow,
            sale.CustomerId,
            "Updated Customer",
            sale.BranchId,
            "Updated Branch",
            [
                (cancelledItem.Id, Guid.NewGuid(), "Cancelled Product Updated", 4, 10m),
                (null, Guid.NewGuid(), "Active Product", 2, 10m)
            ]);

        sale.Items.First(item => item.Id == cancelledItem.Id).Status.Should().Be(SaleStatus.Cancelled);
    }

    [Fact]
    public void Update_ShouldThrow_WhenSaleIsCancelled()
    {
        var sale = CreateSale();
        sale.Cancel();

        var act = () => sale.Update(
            DateTime.UtcNow,
            sale.CustomerId,
            "Updated Customer",
            sale.BranchId,
            "Updated Branch",
            [(sale.Items.First().Id, Guid.NewGuid(), "Updated Product", 4, 10m)]);

        act.Should().Throw<DomainRuleException>()
            .WithMessage("*cannot be changed*");
    }

    private static Sale CreateSale()
    {
        return Sale.Create(
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Customer",
            Guid.NewGuid(),
            "Branch",
            [SaleItem.Create(Guid.NewGuid(), "Product", 4, 10m)]);
    }

    private static Sale CreateSaleWithTwoItems()
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
