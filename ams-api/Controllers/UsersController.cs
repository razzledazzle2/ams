using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using ams_api.Models;
using ams_api.Repositories;
using ams_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ams_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IConfiguration _config;

        public UsersController(
            IUserRepository userRepository,
            JwtService jwtService,
            IRefreshTokenRepository refreshTokenRepo,
            IConfiguration config
        )
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _refreshTokenRepo = refreshTokenRepo;
            _config = config;
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
                Email = request.Email,
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
                    ExpiresAt = DateTime.UtcNow.AddDays(
                        int.Parse(_config["Jwt:RefreshTokenDays"]!)
                    ),
                    CreatedAt = DateTime.UtcNow,
                }
            );
            Response.Cookies.Append(
                "refresh_token",
                refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(
                        int.Parse(_config["Jwt:RefreshTokenDays"]!)
                    ),

                    Path = "/",
                }
            );

            return Ok(new { accessToken = token });
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("Missing refresh token");

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token;

            try
            {
                token = handler.ReadJwtToken(refreshToken);
            }
            catch (Exception ex)
            {
                return Unauthorized("Invalid refresh token");
            }

            // ensure it is a refresh token
            var typeClaim = token.Claims.FirstOrDefault(c => c.Type == "type");
            if (typeClaim?.Value != "refresh")
            {
                return Unauthorized("Invalid token type");
            }

            var userId = Guid.Parse(token.Subject);

            // get the unique id of token
            var jti = Guid.Parse(
                token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value
            );

            var stored = await _refreshTokenRepo.GetByIdAsync(jti);

            // check if refresh token is revoked or expired
            if (stored == null)
            {
                return Unauthorized("Refresh token not found");
            }

            if (stored.RevokedAt != null)
            {
                return Unauthorized("Refresh token revoked");
            }

            if (stored.ExpiresAt < DateTime.UtcNow)
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
                    ExpiresAt = DateTime.UtcNow.AddDays(
                        int.Parse(_config["Jwt:RefreshTokenDays"]!)
                    ),
                    CreatedAt = DateTime.UtcNow,
                }
            );

            Response.Cookies.Append(
                "refresh_token",
                newRefresh,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(
                        int.Parse(_config["Jwt:RefreshTokenDays"]!)
                    ),
                    Path = "/",
                }
            );

            return Ok(new { accessToken = newAccess });
        }
    }
}
