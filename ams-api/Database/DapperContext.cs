namespace ams_api;

using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

public class DapperContext
{
    private readonly IDbConnection _dbConnection;

    public DapperContext(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("PostgressConnection");
        _dbConnection = new NpgsqlConnection(connectionString);
    }

    public IDbConnection DbConnection => _dbConnection;

}
