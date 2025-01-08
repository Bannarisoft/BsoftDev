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
    public class FirstTimePasswordChangeRepository : IChangePassword
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IIPAddressService _ipAddressService;

        public FirstTimePasswordChangeRepository(ApplicationDbContext applicationDbContext,IIPAddressService ipAddressService)
        {
            _applicationDbContext = applicationDbContext;
            _ipAddressService = ipAddressService;
            
        }
        public async Task<PasswordLog> ChangePassword(int userId,PasswordLog passwordLog)
        {
           Console.WriteLine("ChangePassword");
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
                    return passwordLog; 
                }
            }
            else
            {
                return passwordLog;
            }
            throw new Exception("Failed to save changes to the database.");
        }

        public Task<bool> CheckPassword(PasswordLog passwordLog)
        {
            throw new NotImplementedException();
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