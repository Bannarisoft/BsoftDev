using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
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
            await _applicationDbContext.Users.AddAsync(user);
            await _applicationDbContext.SaveChangesAsync();
            return user;
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await _applicationDbContext.Users
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _applicationDbContext.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _applicationDbContext.Users.AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<int> UpdateAsync(int id, User user)
        {
            return await _applicationDbContext.Users
                    .Where(model => model.Id == id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(m => m.Id,user.Id)
                        .SetProperty(m => m.FirstName,user.FirstName)
                        .SetProperty(m => m.LastName,user.LastName)
                        .SetProperty(m => m.UserName,user.UserName)
                        .SetProperty(m => m.UserPassword,user.UserPassword)

                        );
        }
    }
}
