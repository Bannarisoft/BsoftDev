using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Infrastructure.Repositories;
using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BSOFT.Infrastructure.Repositories
{
    public class RoleEntitlementRepository : IRoleEntitlementRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        
        public RoleEntitlementRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task AddAsync(RoleEntitlement roleEntitlement)
        {
            await _applicationDbContext.RoleEntitlement.AddAsync(roleEntitlement);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<List<RoleEntitlement>> GetAllAsync()
        {
            return await _applicationDbContext.RoleEntitlement.Include(x => x.MenuPermissions).ToListAsync();
        }
        
    }
}