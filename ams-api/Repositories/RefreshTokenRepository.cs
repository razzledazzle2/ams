namespace ams_api.Repositories;

using ams_api.Database;
using ams_api.Models;
using Dapper;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly DapperContext _context;

    public RefreshTokenRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(RefreshToken token)
    {
        const string sql =
            @"
            SELECT create_refresh_token(
                @Id,
                @UserId,
                @ExpiresAt,
                @CreatedAt
            );
        ";

        using var conn = _context.CreateConnection();
        await conn.ExecuteAsync(sql, token);
    }

    public async Task<RefreshToken?> GetByIdAsync(Guid id)
    {
        const string sql = "SELECT * FROM get_refresh_token_by_id(@Id);";

        using var conn = _context.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<RefreshToken>(sql, new { Id = id });
    }

    public async Task RevokeAsync(Guid id)
    {
        const string sql = "SELECT revoke_refresh_token(@Id);";

        using var conn = _context.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task RevokeAllForUserAsync(Guid userId)
    {
        const string sql = "SELECT revoke_all_refresh_tokens_for_user(@UserId);";

        using var conn = _context.CreateConnection();
        await conn.ExecuteAsync(sql, new { UserId = userId });
    }
}
