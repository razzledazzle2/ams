namespace ams_api;

using AMS.Api.Database;
using AMS.Api.Models;
using Dapper;

public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;

    public UserRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        const string sql =
            @"
            SELECT *
            FROM users
            WHERE username = @Username;
        ";

        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Username = username });
    }

    public async Task<Guid> CreateUserAsync(User user)
    {
        const string sql =
            @"
        INSERT INTO users (username, password_hash, created_at)
        VALUES (@Username, @PasswordHash, @CreatedAt)
        RETURNING id;
        ";

        using var connection = _context.CreateConnection();
        return await connection.ExecuteScalarAsync<Guid>(sql, user);
    }
}
