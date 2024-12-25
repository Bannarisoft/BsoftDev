using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Application.Common.Interfaces;
using BSOFT.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Repositories
{
    public class UserRoleRepository :IUserRoleRepository
    {
        
        private readonly ApplicationDbContext _applicationDbContext;

    public  UserRoleRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext=applicationDbContext;
    }
    public async Task<List<UserRole>>GetAllRoleAsync()
    {
        
        return await _applicationDbContext.UserRole.ToListAsync();
    }

    public async Task<UserRole> GetByIdAsync(int id)
    {
        return await _applicationDbContext.UserRole.AsNoTracking().FirstOrDefaultAsync(b=>b.Id==id);        
    
    }   
       public async Task<UserRole> CreateAsync(UserRole userrole)
    {
            await _applicationDbContext.UserRole.AddAsync(userrole);
            await _applicationDbContext.SaveChangesAsync();
            return userrole;
    }
     public async Task<int> DeleteAsync(int id)
    {
            var roleToDelete = await _applicationDbContext.UserRole.FirstOrDefaultAsync(u => u.Id == id);
            if (roleToDelete != null)
            {
                _applicationDbContext.UserRole.Remove(roleToDelete);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
    }

     public async Task<int>UpdateAsync(int id, UserRole userrole)
    {
            var existingRole = await _applicationDbContext.UserRole.FirstOrDefaultAsync(u => u.Id == id);
            if (existingRole != null)
            {
                existingRole.RoleName = userrole.RoleName;
                existingRole.Description = userrole.Description;
                existingRole.CompanyId = userrole.CompanyId;
                existingRole.IsActive = userrole.IsActive;                
               
                _applicationDbContext.UserRole.Update(existingRole);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
    }

    public async Task<List<UserRole>> GetRolesAsync(string searchTerm)
    {
        return await _applicationDbContext.UserRole
        .Where(r => EF.Functions.Like(r.RoleName, $"%{searchTerm}%")) // Case-insensitive search
        .Select(r => new UserRole
        {
            Id = r.Id,      
            RoleName = r.RoleName    
        })
        .ToListAsync();
    }


    }
}