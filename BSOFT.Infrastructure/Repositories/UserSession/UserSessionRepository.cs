
using BSOFT.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IUserSession;

namespace BSOFT.Infrastructure.Repositories
{
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;    
        private readonly TimeZoneInfo _indianZone;

        public UserSessionRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;      
             _indianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");         
        }
         private DateTime GetIndianTime()
        {
            // Convert UTC to Indian Standard Time
            DateTime utcNow = DateTime.UtcNow;
            return TimeZoneInfo.ConvertTimeFromUtc(utcNow, _indianZone);
        }
        public async Task AddSessionAsync(UserSessions session)
        {
            _applicationDbContext.UserSession.Add(session);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<UserSessions> GetSessionByJwtIdAsync(string jwtId)
        {
            return await _applicationDbContext.UserSession.FirstOrDefaultAsync(s => s.JwtId == jwtId);
        }

        public async Task UpdateSessionAsync(UserSessions session)
        {
            _applicationDbContext.UserSession.Update(session);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task DeactivateUserSessionsAsync(int userId)
        {
            
            var sessions = await _applicationDbContext.UserSession.Where(s => s.UserId == userId && s.IsActive==1).ToListAsync();
            foreach (var session in sessions)
            {
                session.IsActive = 0;
            }
            await _applicationDbContext.SaveChangesAsync();
        }
        public async Task<UserSessions> GetSessionByUserIdAsync(int userId)
        {
            var indianTime = GetIndianTime();
            return await _applicationDbContext.UserSession
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive == 1 && s.ExpiresAt > indianTime) ;
        }
        public async Task ExpireTokenAsync(string jwtId)
        {
            var session = await _applicationDbContext.UserSession.FirstOrDefaultAsync(s => s.JwtId == jwtId);
            if (session != null)
            {
                session.IsActive = 0;
                await _applicationDbContext.SaveChangesAsync();
            }
        }
        public async Task DeactivateExpiredSessionsAsync()
        {           
            var indianTime = GetIndianTime();
            var expiredSessions = await _applicationDbContext.UserSession
                .Where(s => s.ExpiresAt <= indianTime && s.IsActive==1)
                .ToListAsync();

            foreach (var session in expiredSessions)
            {
                session.IsActive = 0;
            }

            await _applicationDbContext.SaveChangesAsync();
        }
    }
}