using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ams_api.Database;

public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception("Connection string 'DefaultConnection' not found.");
        ;
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}
