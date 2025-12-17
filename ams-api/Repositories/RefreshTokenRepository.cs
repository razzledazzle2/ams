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
        const string sql = @"
            INSERT INTO refresh_tokens (id, user_id, expires_at, created_at)
            VALUES (@Id, @UserId, @ExpiresAt, @CreatedAt);
        ";

        using var conn = _context.CreateConnection();
        await conn.ExecuteAsync(sql, token);
    }

    public async Task<RefreshToken?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT 
                id as Id, 
                user_id as UserId, 
                expires_at as ExpiresAt, 
                revoked_at as RevokedAt, 
                created_at as CreatedAt
            FROM refresh_tokens
            WHERE id = @Id;
        ";

        using var conn = _context.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<RefreshToken>(sql, new { Id = id });
    }

    public async Task RevokeAsync(Guid id)
    {
        const string sql = @"
            UPDATE refresh_tokens
            SET revoked_at = NOW()
            WHERE id = @Id;
        ";

        using var conn = _context.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task RevokeAllForUserAsync(Guid userId)
    {
        const string sql = @"
            UPDATE refresh_tokens
            SET revoked_at = NOW()
            WHERE user_id = @UserId
              AND revoked_at IS NULL;
        ";

        using var conn = _context.CreateConnection();
        await conn.ExecuteAsync(sql, new { UserId = userId });
    }
}