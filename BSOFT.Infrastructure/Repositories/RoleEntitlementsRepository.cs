using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Infrastructure.Repositories;
using BSOFT.Application.Common.Interfaces;
using BSOFT.Domain.Entities;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Repositories
{
    public class RoleEntitlementsRepository : IRoleEntitlementRepository
    {
        private readonly ApplicationDbContext _context;

    public RoleEntitlementsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddRoleEntitlementsAsync(IEnumerable<RoleEntitlement> roleEntitlements)
    {
        await _context.RoleEntitlements.AddRangeAsync(roleEntitlements);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Modules>> GetModulesWithMenusAsync()
    {
        // return await _context.Modules.Include(m => m.menu).ToListAsync();
       return await _context.Set<Modules>().ToListAsync();
    }
    }
}