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
        Task<User?> GetByIdAsync(int userId);
        Task<User> CreateAsync(User user);
        Task<int> UpdateAsync(int userId,User user);
        Task<int> DeleteAsync(int userId,User user);
        Task<User?> GetByUsernameAsync(string username);
    }

}