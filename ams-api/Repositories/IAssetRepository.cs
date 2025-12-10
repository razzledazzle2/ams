namespace ams_api.Repositories;
using ams_api.Models;

public interface IAssetRepository
{
    Task<IEnumerable<Asset>> GetAllAssetsAsync();
    Task<Asset?> GetAssetByIdAsync(Guid id);
    Task<Guid> CreateAssetAsync(Asset asset);
    Task<bool> UpdateAssetAsync(Asset asset);
    Task<bool> DeleteAssetAsync(Guid id);
}
