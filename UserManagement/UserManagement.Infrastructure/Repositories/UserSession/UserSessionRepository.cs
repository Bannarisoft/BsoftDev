
using UserManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IUserSession;
using Infrastructure;
using Core.Application.Common.Interfaces;

namespace UserManagement.Infrastructure.Repositories
{
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;   
        private readonly ITimeZoneService _timeZoneService;       

        public UserSessionRepository(ApplicationDbContext applicationDbContext, ITimeZoneService timeZoneService)
        {
            _applicationDbContext = applicationDbContext;    
            _timeZoneService = timeZoneService;                 
        }       
        public async Task AddSessionAsync(UserSessions session)
        {
            _applicationDbContext.UserSession.Add(session);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<UserSessions> GetSessionByJwtIdAsync(string jwtId)
        {
            return await _applicationDbContext.UserSession.FirstOrDefaultAsync(s => s.JwtId == jwtId)  ?? new UserSessions();
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
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId);
            return await _applicationDbContext.UserSession
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive == 1 && s.ExpiresAt > currentTime) ;
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
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId);
            var expiredSessions = await _applicationDbContext.UserSession
                .Where(s => s.ExpiresAt <= currentTime && s.IsActive==1)
                .ToListAsync();

            foreach (var session in expiredSessions)
            {
                session.IsActive = 0;
            }

            await _applicationDbContext.SaveChangesAsync();
        }
    }
}