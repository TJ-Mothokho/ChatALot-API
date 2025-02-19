using ChatALot.Data.Models.DTOs.User;
using ChatALot.Data.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChatALot.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserServices _user;
        private readonly IConfiguration _config;
        private static readonly Dictionary<string, string> _refreshToken = new();

        public AuthController(IUserServices user, IConfiguration config)
        {
            _user = user;
            _config = config;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateNewUser(CreateUserRequest request)
        {
            try
            {
                Log.Information($"Attempting to add new user: {request.Username}");
                request.Username = request.Username.ToLower();
                var response = await _user.AddUser(request);

                if (response != null)
                {
                    Log.Information($"{request.Username} was successfully created.");
                    return Ok(response);
                }
                else
                {
                    Log.Warning($"User entered bad information when trying to register account!");
                    return BadRequest("Invalid details");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, ex.StackTrace);
                return BadRequest($"Something Went Wrong, Contact Administrator: {ex.Message}");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Log.Information($"{request.Username} is trying to login");

                    var response = await _user.Login(request);

                    if (response == null)
                    {
                        Log.Warning($"{request.Username}: Invalid Username and/or Password");
                        return Unauthorized("Invalid Username or Password");
                    }

                    var accessToken = IssueToken(response);
                    var refreshToken = GenerateRefreshToken();
                    response.Token = accessToken;
                    response.RefreshToken = refreshToken;

                    _refreshToken[response.Id.ToString()] = refreshToken;

                    Log.Information($"{request.Username} successfully logged in");
                    return Ok(response); 
                }
                return BadRequest("Invalid request body.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, ex.StackTrace);
                return BadRequest("Something Went Wrong, Contact Administrator.");
            }
        }

        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var principal = GetPrincipalFromExpiredToken(request.Token);
            if (principal == null)
                return Unauthorized("Invalid Token");

            var userId = principal.FindFirst("Id")?.Value;
            if (userId == null || !_refreshToken.ContainsKey(userId) || _refreshToken[userId] != request.RefreshToken)
                return Unauthorized("Invalid Refresh Token");

            var user = new LoginResponse
            {
                Id = Guid.Parse(userId),
                Username = principal.Identity.Name,
                Email = principal.FindFirst(ClaimTypes.Email)?.Value,
                Role = principal.FindFirst(ClaimTypes.Role)?.Value
            };

            var newAccessToken = IssueToken(user);
            var newRefreshToken = GenerateRefreshToken();

            _refreshToken[userId] = newRefreshToken;

            return Ok(new { Token = newAccessToken, RefreshToken = newRefreshToken });
        }

        [HttpPost("Logout")]
        public IActionResult Logout([FromBody] LogoutRequest request)
        {
            var principal = GetPrincipalFromExpiredToken(request.Token);
            if (principal == null)
                return Unauthorized("Invalid Token");

            var userId = principal.FindFirst("Id")?.Value;
            if (userId != null)
                _refreshToken.Remove(userId); // Remove refresh token on logout

            return Ok("User logged out successfully.");
        }

        [HttpPost("IsLoggedIn")]
        public IActionResult IsLoggedIn([FromBody] string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return BadRequest(false); // No token provided, user is not logged in
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            try
            {
                tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true // Ensures token is not expired
                }, out SecurityToken validatedToken);

                return Ok(true); // Token is valid, user is logged in
            }
            catch
            {
                return Ok(false); // Token is invalid or expired, user is not logged in
            }
        }


        private string IssueToken(LoginResponse user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30), // Shorter expiry for security
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false, // We allow expired tokens for refresh
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public record RefreshTokenRequest(string Token, string RefreshToken);
        public record LogoutRequest(string Token);
    }
}
