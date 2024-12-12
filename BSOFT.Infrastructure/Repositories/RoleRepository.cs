using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Repositories
{
    public class RoleRepository :IRoleRepository
    {
        
        private readonly ApplicationDbContext _applicationDbContext;

    public  RoleRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext=applicationDbContext;
    }
    public async Task<List<Role>>GetAllRoleAsync()
    {
        
        return await _applicationDbContext.Role.ToListAsync();
    }

    public async Task<Role> GetByIdAsync(int id)
    {
        return await _applicationDbContext.Role.AsNoTracking().FirstOrDefaultAsync(b=>b.RoleId==id);        
    
    }   
       public async Task<Role> CreateAsync(Role role)
    {
            await _applicationDbContext.Role.AddAsync(role);
            await _applicationDbContext.SaveChangesAsync();
            return role;
    }
     public async Task<int> DeleteAsync(int id)
    {
            var roleToDelete = await _applicationDbContext.Role.FirstOrDefaultAsync(u => u.RoleId == id);
            if (roleToDelete != null)
            {
                _applicationDbContext.Role.Remove(roleToDelete);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
    }

     public async Task<int>UpdateAsync(int id, Role role)
    {
            var existingRole = await _applicationDbContext.Role.FirstOrDefaultAsync(u => u.RoleId == id);
            if (existingRole != null)
            {
                existingRole.Name = role.Name;
                existingRole.Description = role.Description;
                existingRole.CoId = role.CoId;
                existingRole.IsActive = role.IsActive;
                
                existingRole.ModifiedBy = role.ModifiedBy;
                existingRole.ModifiedAt = role.ModifiedAt;
                existingRole.ModifiedByName=role.ModifiedByName;
                existingRole.ModifiedIP=role.ModifiedIP;

                _applicationDbContext.Role.Update(existingRole);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
    }

    public async Task<List<Role>> GetRolesAsync(string searchTerm)
    {
        return await _applicationDbContext.Role
        .Where(r => EF.Functions.Like(r.Name, $"%{searchTerm}%")) // Case-insensitive search
        .Select(r => new Role
        {
            RoleId = r.RoleId,      
            Name = r.Name    
        })
        .ToListAsync();
    }


    }
}