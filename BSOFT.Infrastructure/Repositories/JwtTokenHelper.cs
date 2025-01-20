using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BSOFT.Infrastructure.Repositories
{
    public class JwtTokenHelper : IJwtTokenHelper
    {
        private readonly JwtSettings _jwtSettings;

        public JwtTokenHelper(IOptions<JwtSettings> jwtSettings)
        {
            if (jwtSettings == null || jwtSettings.Value == null)
            {
                throw new ArgumentNullException(nameof(jwtSettings), "JWT settings are not configured.");
            }

            _jwtSettings = jwtSettings.Value;

            if (string.IsNullOrWhiteSpace(_jwtSettings.SecretKey) || Encoding.UTF8.GetBytes(_jwtSettings.SecretKey).Length < 32)
            {
                throw new ArgumentException("JWT SecretKey must be at least 32 bytes long.", nameof(_jwtSettings.SecretKey));
            }
        }

        public string GenerateToken(string username,int userId,int usertype, IEnumerable<string> roles, out string jti)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username must be provided.", nameof(username));
            }

            // Generate a unique identifier for the token
            jti = Guid.NewGuid().ToString();

            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo indianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");            
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, indianZone);

            // Define claims for the token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.NameId, userId.ToString()) ,
                new Claim(JwtRegisteredClaimNames.Typ, usertype.ToString()) ,
                new Claim(JwtRegisteredClaimNames.Jti, jti),           
                new Claim(JwtRegisteredClaimNames.Exp, indianTime.AddMinutes(_jwtSettings.ExpiryMinutes).ToString())
            };

            // Add roles to claims
            foreach (var role in roles)
            {
                if (!string.IsNullOrWhiteSpace(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            // Create security key and credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the JWT token
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: indianTime.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
        
        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero // Prevent delayed expiration
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException("Invalid token.", ex);
            }
        }
    }
}
