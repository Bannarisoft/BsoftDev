using Core.Domain.Common;
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
        Task<(List<User>,int)> GetAllUsersAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<User?> GetByIdAsync(int userId);      
        Task<List<User>>GetUser(string searchPattern);
        Task<List<string>> GetUserRolesAsync(int userId);
        Task<User?> GetByUsernameAsync(string? username,int? id = null);
        Task<bool> AlreadyExistsAsync(string username,int? id = null);
        Task<User?> GetByUserByUnit(int UserId,int UnitId);
  
    }

}