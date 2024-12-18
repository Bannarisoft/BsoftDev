using BSOFT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Application.Common.Interfaces
{
    using Role = BSOFT.Domain.Entities.Role;
    public interface IRoleRepository
    {
        
        Task<List<Role>> GetAllRoleAsync();
        Task<Role> GetByIdAsync(int id);
        Task<Role> CreateAsync(Role role);
        Task<int> DeleteAsync(int id);
        Task<int> UpdateAsync(int id, Role role);
        Task<List<Role>> GetRolesAsync(string searchTerm);  
      
    }
}