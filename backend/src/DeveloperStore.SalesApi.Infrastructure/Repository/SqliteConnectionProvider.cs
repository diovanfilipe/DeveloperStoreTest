using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace DeveloperStore.SalesApi.Infrastructure.Repository;

public sealed class SqliteConnectionProvider : IDisposable
{
    private readonly string _connectionString;
    private readonly SqliteConnection _keepAliveConnection;

    public SqliteConnectionProvider(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection was not configured.");

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
