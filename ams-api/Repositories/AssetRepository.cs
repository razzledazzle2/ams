namespace ams_api.Repositories;

using ams_api.Database;
using ams_api.Models;
using Dapper;

public class AssetRepository : IAssetRepository
{
    private readonly DapperContext _context;

    public AssetRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Asset>> GetAllAssetsAsync()
    {
        const string sql = "SELECT * FROM get_all_assets();";

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Asset>(sql);
    }

    public async Task<Asset?> GetAssetByIdAsync(Guid id)
    {
        const string sql = "SELECT * FROM get_asset_by_id(@Id);";

        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Asset>(sql, new { Id = id });
    }

    public async Task<Guid> CreateAssetAsync(Asset asset)
    {
        const string sql =
            @"
            SELECT create_asset(
                @Name,
                @Category,
                @Status,
                @Condition,
                @PurchaseDate,
                @Vendor
            );
        ";

        using var connection = _context.CreateConnection();
        return await connection.ExecuteScalarAsync<Guid>(sql, asset);
    }

    public async Task<bool> UpdateAssetAsync(Asset asset)
    {
        const string sql =
            @"
            SELECT update_asset(
                @Id,
                @Name,
                @Category,
                @Status,
                @Condition,
                @PurchaseDate,
                @Vendor
            );
        ";

        using var connection = _context.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>(sql, asset);
    }

    public async Task<bool> DeleteAssetAsync(Guid id)
    {
        const string sql = "SELECT delete_asset(@Id);";

        using var connection = _context.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>(sql, new { Id = id });
    }
}
