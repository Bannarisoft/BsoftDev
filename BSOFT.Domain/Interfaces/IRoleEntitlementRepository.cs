using BSOFT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Domain.Interfaces
{
    public interface IRoleEntitlementRepository
    {
        Task AddAsync(RoleEntitlement roleEntitlement);
        Task<List<RoleEntitlement>> GetAllAsync();
    }

}