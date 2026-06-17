using MySql.Data.MySqlClient;
using System.Data;

namespace TicketingAPI.Database;

public class DatabaseConnection
{
    private readonly string _connectionString;

    public DatabaseConnection(IConfiguration configuration)
    {
        _connectionString = configuration
            .GetConnectionString("DefaultConnection")!;
    }

    public IDbConnection CreateConnection()
        => new MySqlConnection(_connectionString);
}