using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IDepartment
{
    public interface IDepartmentQueryRepository
    {
        
        Task<(IEnumerable<dynamic>, int)> GetAllDepartmentAsync(int PageNumber, int PageSize, string? SearchTerm);
        
        //Task<(List<Department>,int)> GetAllDepartmentAsync(int PageNumber, int PageSize, string? SearchTerm);

        Task<Department> GetByIdAsync(int id);         
        Task<List<Department>> GetAllDepartmentAutoCompleteSearchAsync(string SearchDept);
        Task<bool> FKColumnExistValidation(int Id);
        Task<List<Department>> GetDepartment_SuperAdmin(string SearchDept);

    
         
        
    }
}