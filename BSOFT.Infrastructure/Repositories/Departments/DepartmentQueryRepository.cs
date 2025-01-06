using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IDepartment;

namespace BSOFT.Infrastructure.Repositories.Departments
{
    public class DepartmentQueryRepository :IDepartmentQueryRepository
    { 
    private readonly ApplicationDbContext _applicationDbContext;

    public  DepartmentQueryRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext=applicationDbContext;
    }
    public async Task<List<Department>>GetAllDepartmentAsync()
    {
        
       return await _applicationDbContext.Department.ToListAsync();
        
    }

    public async Task<Department> GetByIdAsync(int id)
    {
        return await _applicationDbContext.Department.AsNoTracking().FirstOrDefaultAsync(b=>b.Id==id);        
    
    }        

  
       public async Task<List<Department>>GetAllDepartmentAutoCompleteAsync()
    {
        
       return await _applicationDbContext.Department.ToListAsync();
        
    }

//   public async Task<List<Department>> GetAllDepartmentAutoCompleteSearchAsync()
//         {
//             return await _applicationDbContext.Department.ToListAsync();
//         }

    public async Task<List<Department>>  GetAllDepartmentAutoCompleteSearchAsync(string SearchDept = null)
        {
                       return await _applicationDbContext.Department
                 .Where(d => EF.Functions.Like(d.DeptName, $"%{SearchDept}%")) 
                 .Select(d => new Department
                 {
                     Id = d.Id,
                     DeptName = d.DeptName
                 })
                 .ToListAsync();
        }
    }
}