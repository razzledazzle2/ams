namespace ams_api.Repositories;

using ams_api.Models;
using Dapper;
using ams_api.Database;

public class AssetRepository : IAssetRepository
{
    private readonly DapperContext _context;
    public AssetRepository(DapperContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Asset>> GetAllAssetsAsync()
    {
        var query = "SELECT * FROM assets";
        using var connection = _context.CreateConnection();
        var assets = await connection.QueryAsync<Asset>(query);
        return assets;
    }

    public async Task<Asset?> GetAssetByIdAsync(Guid id)
    {
        var query = "SELECT * FROM assets WHERE id = @Id";
        using var connection = _context.CreateConnection();
        var asset = await connection.QuerySingleOrDefaultAsync<Asset>(query, new { Id = id });
        return asset;
    }

    public async Task<Guid> CreateAssetAsync(Asset asset)
    {
        var query = @"INSERT INTO assets (name, category, status, condition, purchase_date, vendor, model, image_url, assigned_to, created_at, updated_at) 
                      VALUES (@name, @category, @status, @condition, @purchaseDate, @vendor, @model, @imageUrl, @assignedTo, @createdAt, @updatedAt) 
                      RETURNING id";
        using var connection = _context.CreateConnection();
        var id = await connection.ExecuteScalarAsync<Guid>(query, asset);
        return id;
    }

    public async Task<bool> UpdateAssetAsync(Asset asset)
    {
        var query = @"UPDATE assets SET 
                      name = @name, 
                      category = @category, 
                      status = @status, 
                      condition = @condition, 
                      purchase_date = @purchaseDate, 
                      vendor = @vendor, 
                      model = @model, 
                      image_url = @imageUrl, 
                      assigned_to = @assignedTo, 
                      updated_at = NOW()
                      WHERE id = @id";
        using var connection = _context.CreateConnection();
        var affectedRows = await connection.ExecuteAsync(query, asset);
        return affectedRows > 0;
    }

    public async Task<bool> DeleteAssetAsync(Guid id)
    {
        var query = "DELETE FROM assets WHERE id = @Id";
        using var connection = _context.CreateConnection();
        var affectedRows = await connection.ExecuteAsync(query, new { Id = id });
        return affectedRows > 0;
    }
}
