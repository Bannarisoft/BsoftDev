using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BSOFT.Infrastructure.Repositories
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


         public async Task<string> ChangePassword(int userId,string password, PasswordLog passwordLog)
         {
            
                if (await _applicationDbContext.User.FirstOrDefaultAsync(u => u.UserId == userId) is not { IsFirstTimeUser: true } existingUser)
                   return "User not found or is not a existing user.";

                
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

                    return "Password changed successfully.";
                }

                return "Try different password.";
        }

        public async Task<string> FirstTimeUserChangePassword(int userId,PasswordLog passwordLog)
        {
           
            var existingUser = await _applicationDbContext.User.FirstOrDefaultAsync(u => u.UserId == userId);
            if (existingUser != null && existingUser.IsFirstTimeUser == false)
            {
                existingUser.PasswordHash = passwordLog.PasswordHash;
                existingUser.IsFirstTimeUser =true;
                _applicationDbContext.User.Update(existingUser);
                await _applicationDbContext.SaveChangesAsync();

                 string SystemIp =  _ipAddressService.GetSystemIPAddress();
                 passwordLog.CreatedIP=SystemIp;
                await _applicationDbContext.PasswordLogs.AddAsync(passwordLog);
        
                 var changes = await _applicationDbContext.SaveChangesAsync();
        
                if (changes > 0)
                {
                    return "Password changed successfully."; 
                }
            }
            else
            {
                return "User not found or is not a existing user.";
            }
            return "Failed to save changes to the database.";
        }


        public Task<string> PasswordEncode(string password)
        {
             if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
           
            return Task.FromResult(passwordHash);
        }

    }
}