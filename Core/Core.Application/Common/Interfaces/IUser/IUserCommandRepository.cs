using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IUser
{
    public interface IUserCommandRepository
    {      
        Task<User> CreateAsync(User user);
        Task<int> UpdateAsync(int userId,User user);
        Task<int> DeleteAsync(int userId,User user);   
        // Task<User?> GetByIdAsync(int userId);   
        // Task<User?> GetByUsernameAsync(string username); 
        // Task<List<string>> GetUserRolesAsync(int userId);
    }

}