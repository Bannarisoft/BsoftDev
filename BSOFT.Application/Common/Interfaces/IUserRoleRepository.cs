using BSOFT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Application.Common.Interfaces
{
    using UserRole = BSOFT.Domain.Entities.UserRole;
    public interface IUserRoleRepository
    {
        
        Task<List<UserRole>> GetAllRoleAsync();
        Task<UserRole> GetByIdAsync(int id);
        Task<UserRole> CreateAsync(UserRole userrole);
        Task<int> DeleteAsync(int id);
        Task<int> UpdateAsync(int id, UserRole userrole);
        Task<List<UserRole>> GetRolesAsync(string searchTerm); 
        
         
      
    }
}