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
        Task<User> UpdateAsync(int id,User user);
        Task<User> DeleteAsync(int id);

    }

}