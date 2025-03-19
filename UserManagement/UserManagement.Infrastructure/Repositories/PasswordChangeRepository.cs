using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Infrastructure.Data;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static Core.Domain.Enums.Common.Enums;
using System.Data;
using Dapper;

namespace UserManagement.Infrastructure.Repositories
{
    public class PasswordChangeRepository : IChangePassword
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IIPAddressService _ipAddressService;
        private readonly IDbConnection _dbConnection;

        public PasswordChangeRepository(ApplicationDbContext applicationDbContext,IIPAddressService ipAddressService, IDbConnection dbConnection)
        {
            _applicationDbContext = applicationDbContext;
            _ipAddressService = ipAddressService;
            _dbConnection = dbConnection;
            
        }


         public async Task<bool> ChangePassword(int userId,string password, PasswordLog passwordLog)
         {
            
            var existingUser = await _applicationDbContext.User
             .FirstOrDefaultAsync(u => u.UserId == userId);
               
                 existingUser.PasswordHash = passwordLog.PasswordHash;
                 _applicationDbContext.User.Update(existingUser);
                 passwordLog.CreatedIP =  _ipAddressService.GetSystemIPAddress();
                 await _applicationDbContext.PasswordLogs.AddAsync(passwordLog);
                 await _applicationDbContext.SaveChangesAsync();
                 return true;
                
        }

        public async Task<bool> FirstTimeUserChangePassword(int userId,PasswordLog passwordLog)
        {
           
            var existingUser = await _applicationDbContext.User.FirstOrDefaultAsync(u => u.UserId == userId);
            if (existingUser != null && existingUser.IsFirstTimeUser == FirstTimeUserStatus.Yes)
            {
                existingUser.PasswordHash = passwordLog.PasswordHash;
                existingUser.IsFirstTimeUser =FirstTimeUserStatus.No;
                _applicationDbContext.User.Update(existingUser);
                await _applicationDbContext.SaveChangesAsync();

                 string SystemIp =  _ipAddressService.GetSystemIPAddress();
                 passwordLog.CreatedIP=SystemIp;
                await _applicationDbContext.PasswordLogs.AddAsync(passwordLog);
        
                 var changes = await _applicationDbContext.SaveChangesAsync();
        
                if (changes > 0)
                {
                    return true; 
                }
            }
            else
            {
                return false;
            }
            return false;
        }


            public async Task<string> ResetUserPassword(int userId,PasswordLog passwordLog)
            {
             var existingUser = await _applicationDbContext.User
             .FirstOrDefaultAsync(u => u.UserId == userId);
            // Check if the user exists
            if (existingUser == null)
            {
                return "Username not found.";
            }
            // Check if the user is a first-time user
            if (existingUser.IsFirstTimeUser == FirstTimeUserStatus.Yes)
            {
                return "User is a first-time user. Please complete the initial setup.";
            }
                existingUser.PasswordHash = passwordLog.PasswordHash;
                _applicationDbContext.User.Update(existingUser);
                 await _applicationDbContext.SaveChangesAsync();
                return "Password Reset successfully.";                            
        }



        public Task<string> PasswordEncode(string password)
        {
             if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
           
            return Task.FromResult(passwordHash);
        }

      public async Task<string> GenerateVerificationCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var verificationCode = await Task.Run(() => new string(Enumerable.Repeat(chars, length)
                                    .Select(s => s[random.Next(s.Length)]).ToArray()));
            return verificationCode;
        }

        public async Task<bool> PasswordLog(PasswordLog passwordLog)
        {
            string SystemIp =  _ipAddressService.GetSystemIPAddress();
                 passwordLog.CreatedIP=SystemIp;
                await _applicationDbContext.PasswordLogs.AddAsync(passwordLog);
                return await _applicationDbContext.SaveChangesAsync()>0;
        }
        public async Task<bool> ValidatePassword(int userId,string password)
        {
            var passwordHistoryCount  =  _applicationDbContext.CompanySettings.FirstOrDefault().PasswordHistoryCount;

            if (passwordHistoryCount  == 0)
              return false;

            var sortBy = "Id descending";
                var passwordLogs = await _applicationDbContext.PasswordLogs
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(p => p.Id)
                    .Take(passwordHistoryCount )
                    .ToListAsync();

                    return passwordLogs.Any(log => BCrypt.Net.BCrypt.Verify(password, log.PasswordHash));
        }

        public async Task<bool> ValidateFirstTimeUser(int userId)
        {
             var existingUser = await _applicationDbContext.User
             .FirstOrDefaultAsync(u => u.UserId == userId);
                if (existingUser == null || existingUser.IsFirstTimeUser == FirstTimeUserStatus.Yes)
                {
                    return true;
                }
                return false;
        }
            public async Task<string?> GetUserPasswordHashAsync(int userId)
          {
              return await _applicationDbContext.User
                  .Where(u => u.UserId == userId)
                  .Select(u => u.PasswordHash)
                  .FirstOrDefaultAsync();
          }

        public async Task<bool> ValidatePasswordbyUserName(string username, string password)
        {

        //  var companyId = _ipAddressService.GetCompanyId();
        //   if (companyId == null)
        //     return false;

                var querySettings = @"
                   SELECT top 1 PasswordHistoryCount 
                   FROM [AppData].[CompanySetting] 
                   ";

               var passwordHistoryCount = await _dbConnection.QueryFirstOrDefaultAsync<int?>(querySettings);

               if (passwordHistoryCount == null || passwordHistoryCount == 0)
                   return false; 

               
               var query = @"
                   SELECT TOP(@HistoryCount) pl.PasswordHash
                   FROM [AppSecurity].[PasswordLog] pl
                   INNER JOIN [AppSecurity].[Users] u ON pl.UserId = u.UserId
                   WHERE u.Username = @Username 
                   ORDER BY pl.Id DESC";

               var passwordLogs = await _dbConnection.QueryAsync<string>(query, 
                   new { Username = username,  HistoryCount = passwordHistoryCount });

               
               return passwordLogs.Any(hash => BCrypt.Net.BCrypt.Verify(password, hash));
        }
    }
}