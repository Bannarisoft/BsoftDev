using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BSOFT.Infrastructure.Repositories
{
    public class JwtTokenHelper  : IJwtTokenHelper
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
    public string GenerateToken(string username, IEnumerable<string> roles)
    {
    
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username must be provided.", nameof(username));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add roles as claims
            foreach (var role in roles)
            {
                if (!string.IsNullOrWhiteSpace(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            
    }
    }
}