using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IUser
{
    public interface IUserQueryRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetByIdAsync(int userId);      
        Task<User?> GetByUsernameAsync(string username);
        Task<List<string>> GetUserRolesAsync(int userId);
    }

}