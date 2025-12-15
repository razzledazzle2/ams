using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ams_api.Models;
using ams_api.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace ams_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetRepository _assetRepository;

        public AssetsController(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAssets()
        {
            var assets = await _assetRepository.GetAllAssetsAsync();
            return Ok(assets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAssetById(Guid id)
        {
            var asset = await _assetRepository.GetAssetByIdAsync(id);
            if (asset == null)
            {
                return NotFound();
            }

            return Ok(asset);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsset([FromBody] Asset asset)
        {
            asset.CreatedAt = DateTime.UtcNow;
            asset.UpdatedAt = DateTime.UtcNow;
            var id = await _assetRepository.CreateAssetAsync(asset);
            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Asset asset)
        {
            // if mismatch id, route is correct so use that id 
            asset.Id = id;

            asset.UpdatedAt = DateTime.UtcNow;

            var success = await _assetRepository.UpdateAssetAsync(asset);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _assetRepository.DeleteAssetAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
