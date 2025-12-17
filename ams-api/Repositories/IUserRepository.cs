namespace ams_api.Repositories;
using ams_api.Models;
public interface IUserRepository
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task<Guid> CreateUserAsync(User user);
    Task<User?> GetByIdAsync(Guid id);

}
