using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Infrastructure.Data;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static Core.Domain.Enums.Common.Enums;

namespace UserManagement.Infrastructure.Repositories
{
    public class PasswordChangeRepository : IChangePassword
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IIPAddressService _ipAddressService;

        public PasswordChangeRepository(ApplicationDbContext applicationDbContext,IIPAddressService ipAddressService)
        {
            _applicationDbContext = applicationDbContext;
            _ipAddressService = ipAddressService;
            
        }


         public async Task<bool> ChangePassword(int userId,string password, PasswordLog passwordLog)
         {
            var existingUser = await _applicationDbContext.User
             .FirstOrDefaultAsync(u => u.UserId == userId);
                if (existingUser == null || existingUser.IsFirstTimeUser == FirstTimeUserStatus.Yes)
                {
                    return false;
                }

                
                var sortBy = "Id descending";
                var passwordLogs = await _applicationDbContext.PasswordLogs
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(p => p.Id)
                    .Take(10)
                    .ToListAsync();

              
                if (!passwordLogs.Any(log => BCrypt.Net.BCrypt.Verify(password, log.PasswordHash)))
                {
                    
                    existingUser.PasswordHash = passwordLog.PasswordHash;
                    _applicationDbContext.User.Update(existingUser);

                    passwordLog.CreatedIP =  _ipAddressService.GetSystemIPAddress();
                    await _applicationDbContext.PasswordLogs.AddAsync(passwordLog);
                    await _applicationDbContext.SaveChangesAsync();

                    return true;
                }

                return false;
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
       
    }
}