using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using System.Text;

namespace BSOFT.Infrastructure.Repositories
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
        
            var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        // new Claim(ClaimTypes.Role, user.UserRole.RoleName) // Add role to claims
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);            
        }
    }
}
