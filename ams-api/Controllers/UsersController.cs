using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ams_api.Models;
using ams_api.Repositories;
using ams_api.Services;
namespace ams_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;

        public UsersController(IUserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // check if username exists
            var existing = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (existing != null)
                return BadRequest("Username taken.");

            string hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = hash,
                CreatedAt = DateTime.UtcNow,
            };

            var id = await _userRepository.CreateUserAsync(newUser);

            return Ok(new { id });
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
            var token = _jwtService.GenerateToken(user.Id, user.Username);
            return Ok(new { accessToken = token });
        }
    }
}
