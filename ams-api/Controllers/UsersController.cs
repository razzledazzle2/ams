using ams_api.Models;
using ams_api.Repositories;
using ams_api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace ams_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;

        public UsersController(
            IUserRepository userRepository,
            JwtService jwtService,
            IRefreshTokenRepository refreshTokenRepo
        )
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _refreshTokenRepo = refreshTokenRepo;
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
            // Console.WriteLine("=== LOGIN DEBUG ===");
            // Console.WriteLine($"Username: {user.Username}");
            // Console.WriteLine($"PasswordHash: '{user.PasswordHash}'");

            bool valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!valid)
                return Unauthorized("Invalid username or password.");

            // revoke old tokens
            await _refreshTokenRepo.RevokeAllForUserAsync(user.Id);

            var token = _jwtService.GenerateAccessToken(user.Id, user.Username);
            var refreshTokenId = Guid.NewGuid();

            var refreshToken = _jwtService.GenerateRefreshToken(user.Id, refreshTokenId);
            await _refreshTokenRepo.CreateAsync(
                new RefreshToken
                {
                    Id = refreshTokenId,
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                }
            );
            return Ok(new { accessToken = token, refreshToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
        {
            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken token;
            try
            {
                token = handler.ReadJwtToken(req.RefreshToken);
            }
            catch
            {
                return Unauthorized("Invalid refresh token");
            }

            // ensure it is a refresh token
            if (token.Claims.First(c => c.Type == "type").Value != "refresh")
                return Unauthorized();

            var userId = Guid.Parse(token.Subject);

            // get the unique id of token
            var jti = Guid.Parse(
                token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value
            );

            var stored = await _refreshTokenRepo.GetByIdAsync(jti);

            // check if refresh token is revoked or expired
            if (stored == null || stored.RevokedAt != null || stored.ExpiresAt < DateTime.UtcNow)
            {
                return Unauthorized("Refresh token expired");
            }

            // revoke old one
            await _refreshTokenRepo.RevokeAsync(jti);

            var newRefreshId = Guid.NewGuid();
            var user = await _userRepository.GetByIdAsync(userId);

            var newAccess = _jwtService.GenerateAccessToken(userId, user.Username);
            var newRefresh = _jwtService.GenerateRefreshToken(userId, newRefreshId);

            await _refreshTokenRepo.CreateAsync(
                new RefreshToken
                {
                    Id = newRefreshId,
                    UserId = userId,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                }
            );

            return Ok(new { accessToken = newAccess, refreshToken = newRefresh });
        }
    }
}
