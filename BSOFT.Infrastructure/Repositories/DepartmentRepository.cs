using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Application.Common.Interfaces;
using BSOFT.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Repositories
{
    public class DepartmentRepository :IDepartmentRepository
    { 
    private readonly ApplicationDbContext _applicationDbContext;

    public  DepartmentRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext=applicationDbContext;
    }
    public async Task<List<Department>>GetAllDepartmentAsync()
    {
        
       return await _applicationDbContext.Department.ToListAsync();
        
    }

    public async Task<Department> GetByIdAsync(int id)
    {
        return await _applicationDbContext.Department.AsNoTracking().FirstOrDefaultAsync(b=>b.DeptId==id);        
    
    }        

    public async Task<Department> CreateAsync(Department department)
    {
            await _applicationDbContext.Department.AddAsync(department);
            await _applicationDbContext.SaveChangesAsync();
            return department;
    }

     public async Task<int>UpdateAsync(int id, Department department)
    {
            var existingDept = await _applicationDbContext.Department.FirstOrDefaultAsync(u => u.DeptId == id);
            if (existingDept != null)
            {
                existingDept.ShortName = department.ShortName;
                existingDept.DeptName = department.DeptName;
                existingDept.CoId = department.CoId;
                existingDept.IsActive = department.IsActive;                                

                _applicationDbContext.Department.Update(existingDept);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
    }

    public async Task<int> DeleteAsync(int id ,Department department )
    {
        
            var deptToDelete = await _applicationDbContext.Department.FirstOrDefaultAsync(u => u.DeptId == id);
            if (deptToDelete != null)
            {
               
                deptToDelete.IsActive = department.IsActive;
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
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
                     DeptId = d.DeptId,
                     DeptName = d.DeptName
                 })
                 .ToListAsync();
        }


    



    }
}