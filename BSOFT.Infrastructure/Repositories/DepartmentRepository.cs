using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Domain.Interfaces;
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
                
                existingDept.ModifiedBy = department.ModifiedBy;
                existingDept.ModifiedAt = department.ModifiedAt;
                existingDept.ModifiedByName=department.ModifiedByName;
                existingDept.ModifiedIP=department.ModifiedIP;

                _applicationDbContext.Department.Update(existingDept);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
    }

    public async Task<int> DeleteAsync(int id)
    {
            var deptToDelete = await _applicationDbContext.Department.FirstOrDefaultAsync(u => u.DeptId == id);
            if (deptToDelete != null)
            {
                _applicationDbContext.Department.Remove(deptToDelete);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
    }


    



    }
}