using MySqlConnector;
using System.Data.Common;

namespace TicketingAPI.Database;

public class DatabaseConnection
{
    private readonly string _connectionString;

    public DatabaseConnection(IConfiguration configuration)
    {
        _connectionString = configuration
            .GetConnectionString("DefaultConnection")!;
    }

    public DbConnection CreateConnection()
        => new MySqlConnection(_connectionString);
}