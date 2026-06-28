using Dapper;

namespace DeveloperStore.SalesApi.Infrastructure.Repository;

public sealed class DatabaseInitializer
{
    private readonly SqliteConnectionProvider _connectionProvider;

    public DatabaseInitializer(SqliteConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS sales (
                id TEXT PRIMARY KEY,
                sale_number TEXT NOT NULL,
                sale_date TEXT NOT NULL,
                customer_id TEXT NOT NULL,
                customer_name TEXT NOT NULL,
                branch_id TEXT NOT NULL,
                branch_name TEXT NOT NULL,
                status INTEGER NOT NULL,
                total_sale_amount NUMERIC NOT NULL
            );

            CREATE TABLE IF NOT EXISTS sale_items (
                id TEXT PRIMARY KEY,
                sale_id TEXT NOT NULL,
                product_id TEXT NOT NULL,
                product_name TEXT NOT NULL,
                quantity INTEGER NOT NULL,
                unit_price NUMERIC NOT NULL,
                discount_percent NUMERIC NOT NULL,
                discount_value NUMERIC NOT NULL,
                total_item_amount NUMERIC NOT NULL,
                status INTEGER NOT NULL,
                FOREIGN KEY (sale_id) REFERENCES sales(id)
            );

            CREATE TABLE IF NOT EXISTS idempotency_requests (
                idempotency_key TEXT PRIMARY KEY,
                sale_id TEXT NOT NULL,
                created_at TEXT NOT NULL,
                FOREIGN KEY (sale_id) REFERENCES sales(id)
            );
            """;

        using var connection = _connectionProvider.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));
    }
}
