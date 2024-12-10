using BSOFT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Domain.Interfaces
{
    public interface IRoleRepository
    {
        
        Task<List<Role>> GetAllRoleAsync();
        Task<Role> GetByIdAsync(int id);
        Task<Role> CreateAsync(Role role);
        Task<int> DeleteAsync(int id);
        Task<int> UpdateAsync(int id, Role role);

      
    }
}