namespace ams_api.Repositories;

using ams_api.Models;

public interface IRefreshTokenRepository
{
    Task CreateAsync(RefreshToken token);

    Task<RefreshToken?> GetByIdAsync(Guid id);

    Task RevokeAsync(Guid id);

    Task RevokeAllForUserAsync(Guid userId);
}
