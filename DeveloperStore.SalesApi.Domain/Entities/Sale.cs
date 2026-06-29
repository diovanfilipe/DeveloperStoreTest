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

    public IReadOnlyCollection<SaleItem> Items => _items;

    public static Sale Create(
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        IReadOnlyCollection<SaleItem> items,
        int saleSequence = 1)
    {
        ValidateCreation(saleDate, customerId, customerName, branchId, branchName, items, saleSequence);

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = GenerateSaleNumber(saleDate, saleSequence),
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

    public static Sale Restore(
        Guid id,
        string saleNumber,
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        SaleStatus status,
        IReadOnlyCollection<SaleItem> items)
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
            TotalSaleAmount = 0m
        };

        sale._items.AddRange(items);
        sale.RecalculateTotal();
        return sale;
    }

    public void Update(
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        IReadOnlyCollection<(Guid? ItemId, Guid ProductId, string ProductName, int Quantity, decimal UnitPrice)> items)
    {
        EnsureActive();
        ValidateUpdate(saleDate, customerId, customerName, branchId, branchName, items);

        SaleDate = saleDate;
        CustomerId = customerId;
        CustomerName = customerName.Trim();
        BranchId = branchId;
        BranchName = branchName.Trim();

        var itemsById = _items.ToDictionary(item => item.Id, item => item);
        var updatedItems = new List<SaleItem>();
        var processedItemIds = new HashSet<Guid>();

        foreach (var incomingItem in items)
        {
            if (incomingItem.ItemId is Guid itemId && itemId != Guid.Empty)
            {
                if (!processedItemIds.Add(itemId))
                {
                    throw new DomainRuleException("Sale item identifier cannot be repeated.");
                }

                if (!itemsById.TryGetValue(itemId, out var existingItem))
                {
                    throw new DomainRuleException("Sale item was not found.");
                }

                existingItem.Update(incomingItem.ProductId, incomingItem.ProductName, incomingItem.Quantity, incomingItem.UnitPrice);
                updatedItems.Add(existingItem);
            }
            else
            {
                updatedItems.Add(SaleItem.Create(incomingItem.ProductId, incomingItem.ProductName, incomingItem.Quantity, incomingItem.UnitPrice));
            }
        }

        _items.Clear();
        _items.AddRange(updatedItems);
        RecalculateTotal();
    }

    public IReadOnlyCollection<SaleItem> Cancel()
    {
        if (Status == SaleStatus.Cancelled)
        {
            throw new DomainRuleException("Cancelled sale cannot be cancelled again.");
        }

        var cancelledItems = new List<SaleItem>();
        foreach (var item in _items.Where(currentItem => currentItem.Status == SaleStatus.Active))
        {
            item.Cancel();
            cancelledItems.Add(item);
        }

        RecalculateTotal();
        Status = SaleStatus.Cancelled;

        return cancelledItems;
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

    private void RecalculateTotal()
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

    private static void ValidateCreation(
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        IReadOnlyCollection<SaleItem> items,
        int saleSequence = 1)
    {
        if (saleDate == default)
        {
            throw new DomainRuleException("Sale date is required.");
        }

        if (saleDate > DateTime.UtcNow)
        {
            throw new DomainRuleException("Sale date cannot be greater than the current date.");
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

        if (saleSequence <= 0)
        {
            throw new DomainRuleException("Sale sequence must be greater than zero.");
        }
    }

    private static void ValidateUpdate(
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        IReadOnlyCollection<(Guid? ItemId, Guid ProductId, string ProductName, int Quantity, decimal UnitPrice)> items)
    {
        if (saleDate == default)
        {
            throw new DomainRuleException("Sale date is required.");
        }

        if (saleDate > DateTime.UtcNow)
        {
            throw new DomainRuleException("Sale date cannot be greater than the current date.");
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

    private static string GenerateSaleNumber(DateTime saleDate, int saleSequence)
    {
        return $"SALE-{saleDate:yyyyMMdd}-{saleSequence:D6}";
    }
}
