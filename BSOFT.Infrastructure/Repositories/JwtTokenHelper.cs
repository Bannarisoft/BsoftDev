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

      public string GenerateToken(string username, int userId, int usertype, IEnumerable<string> roles, out string jti)
        {
            jti = Guid.NewGuid().ToString();
            DateTime utcNow = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.NameId, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Typ, usertype.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(utcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var encryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.EncryptionKey));
            var encryptingCredentials = new EncryptingCredentials(encryptionKey, SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = utcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(encryptedToken);
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
