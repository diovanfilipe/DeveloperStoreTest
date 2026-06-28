using Microsoft.Data.Sqlite;

namespace DeveloperStore.SalesApi.Infrastructure.Repository;

public sealed class SqliteConnectionProvider : IDisposable
{
    private readonly string _connectionString = "Data Source=DeveloperStoreSales;Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _keepAliveConnection;

    public SqliteConnectionProvider()
    {
        _keepAliveConnection = new SqliteConnection(_connectionString);
        _keepAliveConnection.Open();
    }

    public SqliteConnection CreateConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    public void Dispose()
    {
        _keepAliveConnection.Dispose();
    }
}
