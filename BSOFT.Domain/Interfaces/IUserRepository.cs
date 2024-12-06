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
        Task<User> GetByIdAsync(int id);
        Task<User> CreateAsync(User user);
        Task<int> UpdateAsync(int id,User user);
        Task<int> DeleteAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        // Task<User> ValidateUserAsync(string username, string PasswordHash);

    }

}