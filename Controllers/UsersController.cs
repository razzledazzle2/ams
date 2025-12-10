using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepository.GetUserByUsernameAsync(request.Username);

            if (user == null)
                return Unauthorized("Invalid username or password.");

            bool valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!valid)
                return Unauthorized("Invalid username or password.");

            return Ok(
                new
                {
                    user.Id,
                    user.Username,
                    user.Role,
                }
            );
        }
    }
}
