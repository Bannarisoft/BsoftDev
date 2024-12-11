using BSOFT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Domain.Interfaces
{
    public interface IDepartmentRepository
    {
        
        Task<List<Department>> GetAllDepartmentAsync();
        Task<Department> GetByIdAsync(int id);
        Task<Department> CreateAsync(Department department);

         Task<int> UpdateAsync(int id, Department department);
        
         Task<int> DeleteAsync(int id,Department department);
    }
}