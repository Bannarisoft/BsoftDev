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

        public async Task<int> DeleteAsync(Guid id)
        {
            return await _applicationDbContext.User
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _applicationDbContext.User.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _applicationDbContext.User.AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<int> UpdateAsync(Guid id, User user)
        {
            // return await _applicationDbContext.Users
            //         .Where(model => model.Id == id)
            //         .ExecuteUpdateAsync(setters => setters
            //             .SetProperty(m => m.Id,user.Id)
            //             .SetProperty(m => m.FirstName,user.FirstName)
            //             .SetProperty(m => m.LastName,user.LastName)
            //             .SetProperty(m => m.UserName,user.UserName)
            //             .SetProperty(m => m.UserPassword,user.UserPassword)

            //             );
            var existingUser = await _applicationDbContext.User.FirstOrDefaultAsync(u => u.Id == id);
            if (existingUser != null)
            {
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
