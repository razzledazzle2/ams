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
            configuration.GetConnectionString("PostgressConnection")
            ?? throw new Exception("Connection string 'PostgressConnection' not found.");
        ;
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}
