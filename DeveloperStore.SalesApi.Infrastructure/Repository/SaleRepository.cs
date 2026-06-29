using Dapper;
using DeveloperStore.SalesApi.Domain.Entities;
using DeveloperStore.SalesApi.Domain.Enums;
using DeveloperStore.SalesApi.Domain.Repositories;
using DeveloperStore.SalesApi.Infrastructure.Repository.Models;

namespace DeveloperStore.SalesApi.Infrastructure.Repository;

public sealed class SaleRepository : ISaleRepository
{
    private readonly SqliteConnectionProvider _connectionProvider;

    public SaleRepository(SqliteConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<List<Sale>> GetAllAsync(CancellationToken cancellationToken)
    {
        using var connection = _connectionProvider.CreateConnection();

        var saleRows = (await connection.QueryAsync<SaleRow>(new CommandDefinition(
            """
            SELECT id, sale_number AS SaleNumber, sale_date AS SaleDate, customer_id AS CustomerId,
                   customer_name AS CustomerName, branch_id AS BranchId, branch_name AS BranchName,
                   status, total_sale_amount AS TotalSaleAmount
            FROM sales
            ORDER BY sale_date DESC
            """,
            cancellationToken: cancellationToken))).ToList();

        if (saleRows.Count == 0)
        {
            return [];
        }

        var itemRows = (await connection.QueryAsync<SaleItemRow>(new CommandDefinition(
            """
            SELECT id, sale_id AS SaleId, product_id AS ProductId, product_name AS ProductName,
                   quantity, unit_price AS UnitPrice, discount_percent AS DiscountPercent,
                   discount_value AS DiscountValue, total_item_amount AS TotalItemAmount, status
            FROM sale_items
            """,
            cancellationToken: cancellationToken))).ToList();

        return MapSales(saleRows, itemRows);
    }

    public async Task<Sale?> GetByIdAsync(Guid saleId, CancellationToken cancellationToken)
    {
        using var connection = _connectionProvider.CreateConnection();

        var saleRow = await connection.QuerySingleOrDefaultAsync<SaleRow>(new CommandDefinition(
            """
            SELECT id, sale_number AS SaleNumber, sale_date AS SaleDate, customer_id AS CustomerId,
                   customer_name AS CustomerName, branch_id AS BranchId, branch_name AS BranchName,
                   status, total_sale_amount AS TotalSaleAmount
            FROM sales
            WHERE id = @SaleId
            """,
            new { SaleId = saleId.ToString() },
            cancellationToken: cancellationToken));

        if (saleRow is null)
        {
            return null;
        }

        var itemRows = (await connection.QueryAsync<SaleItemRow>(new CommandDefinition(
            """
            SELECT id, sale_id AS SaleId, product_id AS ProductId, product_name AS ProductName,
                   quantity, unit_price AS UnitPrice, discount_percent AS DiscountPercent,
                   discount_value AS DiscountValue, total_item_amount AS TotalItemAmount, status
            FROM sale_items
            WHERE sale_id = @SaleId
            """,
            new { SaleId = saleId.ToString() },
            cancellationToken: cancellationToken))).ToList();

        return MapSale(saleRow, itemRows);
    }

    public async Task<int> GetNextSaleSequenceAsync(DateTime saleDate, CancellationToken cancellationToken)
    {
        using var connection = _connectionProvider.CreateConnection();

        var start = saleDate.Date;
        var end = start.AddDays(1);

        var count = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            """
            SELECT COUNT(1)
            FROM sales
            WHERE sale_date >= @Start
              AND sale_date < @End
            """,
            new
            {
                Start = start,
                End = end
            },
            cancellationToken: cancellationToken));

        return count + 1;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken)
    {
        using var connection = _connectionProvider.CreateConnection();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO sales (id, sale_number, sale_date, customer_id, customer_name, branch_id, branch_name, status, total_sale_amount)
            VALUES (@Id, @SaleNumber, @SaleDate, @CustomerId, @CustomerName, @BranchId, @BranchName, @Status, @TotalSaleAmount)
            """,
            new
            {
                Id = sale.Id.ToString(),
                sale.SaleNumber,
                sale.SaleDate,
                CustomerId = sale.CustomerId.ToString(),
                sale.CustomerName,
                BranchId = sale.BranchId.ToString(),
                sale.BranchName,
                Status = (int)sale.Status,
                sale.TotalSaleAmount
            },
            transaction,
            cancellationToken: cancellationToken));

        foreach (var item in sale.Items)
        {
            await InsertItemAsync(connection, transaction, sale.Id, item, cancellationToken);
        }

        transaction.Commit();
        return sale;
    }

    public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken)
    {
        using var connection = _connectionProvider.CreateConnection();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition(
            """
            UPDATE sales
            SET sale_date = @SaleDate,
                customer_id = @CustomerId,
                customer_name = @CustomerName,
                branch_id = @BranchId,
                branch_name = @BranchName,
                status = @Status,
                total_sale_amount = @TotalSaleAmount
            WHERE id = @Id
            """,
            new
            {
                Id = sale.Id.ToString(),
                sale.SaleDate,
                CustomerId = sale.CustomerId.ToString(),
                sale.CustomerName,
                BranchId = sale.BranchId.ToString(),
                sale.BranchName,
                Status = (int)sale.Status,
                sale.TotalSaleAmount
            },
            transaction,
            cancellationToken: cancellationToken));

        await connection.ExecuteAsync(new CommandDefinition(
            "DELETE FROM sale_items WHERE sale_id = @SaleId",
            new { SaleId = sale.Id.ToString() },
            transaction,
            cancellationToken: cancellationToken));

        foreach (var item in sale.Items)
        {
            await InsertItemAsync(connection, transaction, sale.Id, item, cancellationToken);
        }

        transaction.Commit();
    }

    private static async Task InsertItemAsync(
        System.Data.Common.DbConnection connection,
        System.Data.Common.DbTransaction transaction,
        Guid saleId,
        SaleItem item,
        CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO sale_items
                (id, sale_id, product_id, product_name, quantity, unit_price, discount_percent, discount_value, total_item_amount, status)
            VALUES
                (@Id, @SaleId, @ProductId, @ProductName, @Quantity, @UnitPrice, @DiscountPercent, @DiscountValue, @TotalItemAmount, @Status)
            """,
            new
            {
                Id = item.Id.ToString(),
                SaleId = saleId.ToString(),
                ProductId = item.ProductId.ToString(),
                item.ProductName,
                item.Quantity,
                item.UnitPrice,
                item.DiscountPercent,
                item.DiscountValue,
                item.TotalItemAmount,
                Status = (int)item.Status
            },
            transaction,
            cancellationToken: cancellationToken));
    }

    private static List<Sale> MapSales(List<SaleRow> saleRows, List<SaleItemRow> itemRows)
    {
        var itemsBySaleId = itemRows.GroupBy(item => item.SaleId).ToDictionary(group => group.Key, group => group.ToList());

        return saleRows.Select(row =>
        {
            itemsBySaleId.TryGetValue(row.Id, out var saleItems);
            return MapSale(row, saleItems ?? []);
        }).ToList();
    }

    private static Sale MapSale(SaleRow saleRow, List<SaleItemRow> itemRows)
    {
        return Sale.Restore(
            Guid.Parse(saleRow.Id),
            saleRow.SaleNumber,
            saleRow.SaleDate,
            Guid.Parse(saleRow.CustomerId),
            saleRow.CustomerName,
            Guid.Parse(saleRow.BranchId),
            saleRow.BranchName,
            (SaleStatus)saleRow.Status,
            itemRows.Select(item => SaleItem.Restore(
                Guid.Parse(item.Id),
                Guid.Parse(item.ProductId),
                item.ProductName,
                item.Quantity,
                item.UnitPrice,
                item.DiscountPercent,
                item.DiscountValue,
                item.TotalItemAmount,
                (SaleStatus)item.Status)).ToList(),
            saleRow.TotalSaleAmount);
    }
}
