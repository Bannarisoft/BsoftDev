using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;

namespace BSOFT.Infrastructure.Repositories
{
    public class FirstTimePasswordChangeRepository : IChangePassword
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public FirstTimePasswordChangeRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            
        }
        public async Task<PasswordLog> ChangePassword(PasswordLog passwordLog)
        {
            
           await _applicationDbContext.PasswordLogs.AddAsync(passwordLog);
        
            var changes = await _applicationDbContext.SaveChangesAsync();
        
             if (changes > 0)
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