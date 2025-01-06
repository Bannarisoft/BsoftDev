using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IUserRole
{
    using UserRole = Core.Domain.Entities.UserRole;
    public interface IUserRoleQueryRepository
    {
        
        Task<List<UserRole>> GetAllRoleAsync();
        Task<UserRole> GetByIdAsync(int id);
        Task<List<UserRole>> GetRolesAsync(string searchTerm); 
        
         
      
    }
}