namespace ams_api;

using AMS.Api.Models;
using Dapper;
using AMS.Api.Database;

public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;

    public UserRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        const string sql = @"
            SELECT *
            FROM users
            WHERE username = @Username;
        ";

        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Username = username });
    }
}
