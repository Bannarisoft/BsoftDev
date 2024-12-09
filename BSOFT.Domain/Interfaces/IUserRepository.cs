using BSOFT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<User> CreateAsync(User user);
        Task<int> UpdateAsync(Guid id,User user);
        Task<int> DeleteAsync(Guid id);
        Task<User?> GetByUsernameAsync(string username);
        // Task<User> ValidateUserAsync(string username, string PasswordHash);

    }

}