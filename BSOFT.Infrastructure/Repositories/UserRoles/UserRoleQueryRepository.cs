using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUserRole;

namespace BSOFT.Infrastructure.Repositories.UserRoles
{
    public class UserRoleQueryRepository :IUserRoleQueryRepository
    {
        
        private readonly ApplicationDbContext _applicationDbContext;

    public  UserRoleQueryRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext=applicationDbContext;
    }
    public async Task<List<UserRole>>GetAllRoleAsync()
    {
        
        return await _applicationDbContext.UserRole.ToListAsync();
    }

    public async Task<UserRole?> GetByIdAsync(int id)
    {
            return await _applicationDbContext.UserRole.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

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