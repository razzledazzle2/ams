namespace ams_api;
using AMS.Api.Models;
public interface IUserRepository
{
    Task<User?> GetUserByUsernameAsync(string username);
}
