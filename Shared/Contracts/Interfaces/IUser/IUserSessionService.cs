using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Models.Users;

namespace Contracts.Interfaces.IUser
{
    public interface IUserSessionService
    {
        Task<UserSessionDto?> GetSessionByJwtIdAsync(string jwtId,string token);
        Task<bool> UpdateSessionAsync(string jwtId, DateTimeOffset lastActivity,string token);
    }
}