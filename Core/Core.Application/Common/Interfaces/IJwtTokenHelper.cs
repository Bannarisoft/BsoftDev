using System.Security.Claims;

namespace Core.Application.Common.Interfaces
{
    public interface IJwtTokenHelper
    {
        string GenerateToken(string username,int userid,int usertype, IEnumerable<string> roles, out string jti);
        ClaimsPrincipal ValidateToken(string token);
    }
}