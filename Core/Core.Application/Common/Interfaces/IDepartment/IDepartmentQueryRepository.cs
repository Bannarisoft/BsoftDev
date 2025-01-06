using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IDepartment
{
    public interface IDepartmentQueryRepository
    {
        
        Task<List<Department>> GetAllDepartmentAsync();
        Task<Department> GetByIdAsync(int id);           
        Task<List<Department>> GetAllDepartmentAutoCompleteAsync();

        Task<List<Department>> GetAllDepartmentAutoCompleteSearchAsync(string SearchDept);
        
    }
}