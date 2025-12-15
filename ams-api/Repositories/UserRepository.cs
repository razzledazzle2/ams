namespace ams_api.Repositories;

using ams_api.Database;
using ams_api.Models;
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
            SELECT 
                id,
                username,
                password_hash AS ""PasswordHash"",
                created_at   AS ""CreatedAt""
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
