using System.Security.Claims;
using Core.Domain.Common;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces
{
    public interface IJwtTokenHelper
    {
        string GenerateToken(string? username,int userid,int usertype, IEnumerable<string> roles,string? mobile,string? email,int unitId,List<UserCompany> companyId, out string jti);
        ClaimsPrincipal ValidateToken(string token);
    }
}