using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using BSOFT.Infrastructure.Data;
using System.Data;
using Core.Application.Common.Interfaces.INotifications;

namespace BSOFT.Infrastructure.Repositories.Notifications
{
    public class NotificationsQueryRepository : INotificationsQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        public NotificationsQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async  Task<DateTime?> GetLastPasswordChangeDate(string username)
        {
            var query = @"
            SELECT TOP 1 a.CreatedAt as PasswordLastChangeDate 
            FROM AppSecurity.PasswordLog a 
            JOIN AppSecurity.Users b ON a.UserName = b.UserName AND a.UserId = b.UserId 
            WHERE a.UserName = @username AND b.IsFirstTimeUser = 1 
            ORDER BY a.CreatedAt DESC";

            var result = await _dbConnection.QueryFirstOrDefaultAsync<DateTime?>(query, new { username });
            return result; // Keep it nullable
            
        }

        public async Task<(int PwdExpiryDays, int PwdExpiryAlertDays)> GetPasswordExpiryDays()
        {
             var query = @"SELECT PasswordExpiryDays, PasswordExpiryAlertDays FROM AppSecurity.AdminSecuritySettings";
             var result = await _dbConnection.QueryFirstOrDefaultAsync<(int PwdExpiryDays, int PwdExpiryAlertDays)>(query);
             return result;
        }
    }

    
}