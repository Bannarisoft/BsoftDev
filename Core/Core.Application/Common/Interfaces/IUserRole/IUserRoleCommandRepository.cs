using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IUserRole
{
    using UserRole = Core.Domain.Entities.UserRole;
    public interface IUserRoleCommandRepository
    {
        
        Task<UserRole> CreateAsync(UserRole userrole);
        Task<int> DeleteAsync(int id, UserRole userrole);
        Task<int> UpdateAsync(int id, UserRole userrole);
    }
}