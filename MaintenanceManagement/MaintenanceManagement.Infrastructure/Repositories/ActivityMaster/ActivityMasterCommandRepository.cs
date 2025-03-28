using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IActivityMaster;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;

namespace MaintenanceManagement.Infrastructure.Repositories.ActivityMaster
{
    public class ActivityMasterCommandRepository :IActivityMasterCommandRepository
    {
         private readonly ApplicationDbContext _dbContext;       

          public ActivityMasterCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }

        public async   Task<Core.Domain.Entities.ActivityMaster> CreateAsync(Core.Domain.Entities.ActivityMaster  activityMaster)
        {            
             await _dbContext.ActivityMaster.AddAsync(activityMaster);
                await _dbContext.SaveChangesAsync();
                return activityMaster;
        } 
        
    }
}