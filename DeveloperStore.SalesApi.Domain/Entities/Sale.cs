using DeveloperStore.SalesApi.Domain.Enums;
using DeveloperStore.SalesApi.Domain.Exceptions;

namespace DeveloperStore.SalesApi.Domain.Entities;

public sealed class Sale
{
    private readonly List<SaleItem> _items = [];

    private Sale()
    {
    }

    public Guid Id { get; private set; }

    public string SaleNumber { get; private set; } = string.Empty;

    public DateTime SaleDate { get; private set; }

    public Guid CustomerId { get; private set; }

    public string CustomerName { get; private set; } = string.Empty;

    public Guid BranchId { get; private set; }

    public string BranchName { get; private set; } = string.Empty;

    public SaleStatus Status { get; private set; }

    public decimal TotalSaleAmount { get; private set; }

    public List<SaleItem> Items => _items;

    public static Sale Create(
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        List<SaleItem> items)
    {
        Validate(saleDate, customerId, customerName, branchId, branchName, items);

        var saleId = Guid.NewGuid();

        var sale = new Sale
        {
            Id = saleId,
            SaleNumber = saleId.ToString("N"),
            SaleDate = saleDate,
            CustomerId = customerId,
            CustomerName = customerName.Trim(),
            BranchId = branchId,
            BranchName = branchName.Trim(),
            Status = SaleStatus.Active
        };

        sale._items.AddRange(items);
        sale.RecalculateTotal();
        return sale;
    }

    public static Sale Rehydrate(
        Guid id,
        string saleNumber,
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        SaleStatus status,
        List<SaleItem> items,
        decimal totalSaleAmount)
    {
        var sale = new Sale
        {
            Id = id,
            SaleNumber = saleNumber,
            SaleDate = saleDate,
            CustomerId = customerId,
            CustomerName = customerName,
            BranchId = branchId,
            BranchName = branchName,
            Status = status,
            TotalSaleAmount = totalSaleAmount
        };

        sale._items.AddRange(items);
        return sale;
    }

    public void Update(
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        List<SaleItem> items)
    {
        EnsureActive();
        Validate(saleDate, customerId, customerName, branchId, branchName, items);

        SaleDate = saleDate;
        CustomerId = customerId;
        CustomerName = customerName.Trim();
        BranchId = branchId;
        BranchName = branchName.Trim();

        _items.Clear();
        _items.AddRange(items);
        RecalculateTotal();
    }

    public void Cancel()
    {
        if (Status == SaleStatus.Cancelled)
        {
            throw new DomainRuleException("Cancelled sale cannot be cancelled again.");
        }

        Status = SaleStatus.Cancelled;
    }

    public void CancelItem(Guid itemId)
    {
        EnsureActive();

        var item = _items.SingleOrDefault(currentItem => currentItem.Id == itemId);
        if (item is null)
        {
            throw new DomainRuleException("Sale item was not found.");
        }

        item.Cancel();
        RecalculateTotal();
    }

    public void RecalculateTotal()
    {
        TotalSaleAmount = _items
            .Where(item => item.Status == SaleStatus.Active)
            .Sum(item => item.TotalItemAmount);
    }

    private void EnsureActive()
    {
        if (Status == SaleStatus.Cancelled)
        {
            throw new DomainRuleException("Cancelled sale cannot be changed.");
        }
    }

    private static void Validate(
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        List<SaleItem> items)
    {
        if (saleDate == default)
        {
            throw new DomainRuleException("Sale date is required.");
        }

        if (customerId == Guid.Empty)
        {
            throw new DomainRuleException("CustomerId is required.");
        }

        if (branchId == Guid.Empty)
        {
            throw new DomainRuleException("BranchId is required.");
        }

        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new DomainRuleException("CustomerName is required.");
        }

        if (string.IsNullOrWhiteSpace(branchName))
        {
            throw new DomainRuleException("BranchName is required.");
        }

        if (items.Count == 0)
        {
            throw new DomainRuleException("Sale must contain at least one item.");
        }
    }
}
