using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IAdminSecuritySettings;

namespace BSOFT.Infrastructure.Repositories.AdminSecuritySettings
{
    public class AdminSecuritySettingsCommandRepository  : IAdminSecuritySettingsCommandRepository 
    {
        private readonly ApplicationDbContext _applicationDbContext;

         public  AdminSecuritySettingsCommandRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext=applicationDbContext;
    } 
     public async Task<Core.Domain.Entities.AdminSecuritySettings> CreateAsync(Core.Domain.Entities.AdminSecuritySettings adminSecuritySettings)
    {
            await _applicationDbContext.AdminSecuritySettings.AddAsync(adminSecuritySettings);
            await _applicationDbContext.SaveChangesAsync();
            return adminSecuritySettings;
    }

        
    }
}