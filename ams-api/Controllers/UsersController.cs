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

        private int RefreshExpiryMinutes =>
            int.TryParse(_config["Jwt:TestingRefreshMinutes"], out var mins) ? mins : 2; // fallback (1 day)

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

            var refreshExpiresAt = DateTime.UtcNow.AddMinutes(RefreshExpiryMinutes);
            await _refreshTokenRepo.CreateAsync(
                new RefreshToken
                {
                    Id = refreshTokenId,
                    UserId = user.Id,
                    // ExpiresAt = DateTime.UtcNow.AddDays(
                    //     int.Parse(_config["Jwt:RefreshTokenDays"]!)
                    // ),
                    ExpiresAt = refreshExpiresAt,

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
                    // Expires = DateTimeOffset.UtcNow.AddDays(
                    //     int.Parse(_config["Jwt:RefreshTokenDays"]!)
                    // ),
                    Expires = refreshExpiresAt,

                    Path = "/",
                }
            );

            return Ok(new { accessToken = token });
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            Console.WriteLine("=== REFRESH ENDPOINT HIT ===");
            Console.WriteLine("Cookies received:");

            foreach (var c in Request.Cookies)
            {
                Console.WriteLine($"{c.Key} = {c.Value}");
            }
            Console.WriteLine($"Cookie count: {Request.Cookies.Count}");
            Console.WriteLine(
                $"refresh_token present: {Request.Cookies.ContainsKey("refresh_token")}"
            );

            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("Missing refresh token");

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token;

            try
            {
                token = handler.ReadJwtToken(refreshToken);
                Console.WriteLine($"JWT decoded successfully");
                Console.WriteLine($"JWT exp: {token.ValidTo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JWT decode failed: {ex.Message}");
                return Unauthorized("Invalid refresh token");
            }

            // ensure it is a refresh token
            var typeClaim = token.Claims.FirstOrDefault(c => c.Type == "type");
            if (typeClaim?.Value != "refresh")
            {
                Console.WriteLine($"Not a refresh token. Type: {typeClaim?.Value}");
                return Unauthorized("Invalid token type");
            }

            var userId = Guid.Parse(token.Subject);
            Console.WriteLine($"User ID: {userId}");

            // get the unique id of token
            var jti = Guid.Parse(
                token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value
            );
            Console.WriteLine($"Token JTI: {jti}");

            var stored = await _refreshTokenRepo.GetByIdAsync(jti);

            Console.WriteLine($"=== DATABASE CHECK ===");
            Console.WriteLine($"Stored token found: {stored != null}");
            if (stored != null)
            {
                Console.WriteLine($"Token revoked: {stored.RevokedAt != null}");
                Console.WriteLine(
                    $"Token expires at (DB): {stored.ExpiresAt:yyyy-MM-dd HH:mm:ss} UTC"
                );
                Console.WriteLine(
                    $"Current UTC time:      {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC"
                );
                Console.WriteLine($"Token expired: {stored.ExpiresAt < DateTime.UtcNow}");
            }

            // check if refresh token is revoked or expired
            if (stored == null)
            {
                Console.WriteLine("FAIL: Token not found in database");
                return Unauthorized("Refresh token not found");
            }

            if (stored.RevokedAt != null)
            {
                Console.WriteLine("FAIL: Token has been revoked");
                return Unauthorized("Refresh token revoked");
            }

            if (stored.ExpiresAt < DateTime.UtcNow)
            {
                Console.WriteLine("FAIL: Token has expired");
                return Unauthorized("Refresh token expired");
            }

            Console.WriteLine("SUCCESS: Token is valid, generating new tokens");

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
                    ExpiresAt = DateTime.UtcNow.AddMinutes(RefreshExpiryMinutes),
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
                    Expires = DateTimeOffset.UtcNow.AddMinutes(RefreshExpiryMinutes),
                    Path = "/",
                }
            );

            Console.WriteLine("New tokens generated successfully");
            return Ok(new { accessToken = newAccess });
        }
    }
}
