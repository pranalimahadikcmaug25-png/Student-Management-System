using Microsoft.IdentityModel.Tokens;
using StudentManagementSystem.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentManagementSystem.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generates a signed JWT token for the given user.
        /// Returns the token string and its expiration time.
        /// </summary>
        public (string Token, DateTime Expiration) GenerateToken(User user)
        {
            var jwtSettings  = _configuration.GetSection("JwtSettings");
            var secretKey    = jwtSettings["SecretKey"]
                               ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
            var issuer       = jwtSettings["Issuer"]       ?? "StudentManagementSystem";
            var audience     = jwtSettings["Audience"]     ?? "StudentManagementSystem";
            var expiryHours  = int.TryParse(jwtSettings["ExpiryHours"], out var h) ? h : 24;

            var key          = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials  = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration   = DateTime.UtcNow.AddHours(expiryHours);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,  user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                          DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                          ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer:             issuer,
                audience:           audience,
                claims:             claims,
                notBefore:          DateTime.UtcNow,
                expires:            expiration,
                signingCredentials: credentials);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
        }
    }
}
