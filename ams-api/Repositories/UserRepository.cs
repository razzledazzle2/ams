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
        const string sql = "SELECT * FROM get_user_by_username(@Username);";

        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Username = username });
    }

    public async Task<Guid> CreateUserAsync(User user)
    {
        const string sql =
            @"
            SELECT create_user(
                @Username,
                @Email,
                @PasswordHash
            );
            ";

        using var connection = _context.CreateConnection();
        return await connection.ExecuteScalarAsync<Guid>(sql, user);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        const string sql = "SELECT * FROM get_user_by_id(@Id);";

        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
    }
}
