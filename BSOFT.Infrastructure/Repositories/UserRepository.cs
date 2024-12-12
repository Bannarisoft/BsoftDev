using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Infrastructure.Repositories;
using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UserRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<User> CreateAsync(User user)
        {
            await _applicationDbContext.User.AddAsync(user);
            await _applicationDbContext.SaveChangesAsync();
            return user;
        }

        public async Task<int> DeleteAsync(int userId,User user)
        {
            var existingUser = await _applicationDbContext.User.FirstOrDefaultAsync(u => u.UserId == userId);
            if (existingUser != null)
            {
                existingUser.IsActive = user.IsActive;
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _applicationDbContext.User.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _applicationDbContext.User.AsNoTracking()
                .FirstOrDefaultAsync(b => b.UserId == userId);
        }

        public async Task<int> UpdateAsync(int userId, User user)
        {
            var existingUser = await _applicationDbContext.User.FirstOrDefaultAsync(u => u.UserId == userId);
            if (existingUser != null)
            {
                existingUser.UserId = user.UserId;
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.UserName = user.UserName;
                existingUser.PasswordHash = user.PasswordHash;
                existingUser.UserType = user.UserType;
                existingUser.Mobile = user.Mobile;
                existingUser.EmailId = user.EmailId;
                existingUser.CoId = user.CoId;
                existingUser.DivId = user.DivId;
                existingUser.UnitId = user.UnitId;
                existingUser.RoleId = user.RoleId;
                existingUser.Role = user.Role;
                existingUser.IsFirstTimeUser = user.IsFirstTimeUser;

                _applicationDbContext.User.Update(existingUser);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _applicationDbContext.User.FirstOrDefaultAsync(u => u.UserName == username);
        }
        
    }
}
