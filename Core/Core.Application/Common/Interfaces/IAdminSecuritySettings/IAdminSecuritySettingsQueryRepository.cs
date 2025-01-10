using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IAdminSecuritySettings
{
    public interface IAdminSecuritySettingsQueryRepository 
    {
         Task<List<Core.Domain.Entities.AdminSecuritySettings>> GetAllAdminSecuritySettingsAsync();
         



    }
}