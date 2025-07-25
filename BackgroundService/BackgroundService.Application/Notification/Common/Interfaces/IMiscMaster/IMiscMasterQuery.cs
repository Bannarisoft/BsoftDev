using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Entities.Notification;

namespace BackgroundService.Application.Notification.Common.Interfaces.IMiscMaster
{
    public interface IMiscMasterQuery
    {
        Task<MiscMaster> GetMiscMasterByName(string miscTypeCode, string miscTypeName);  
    }
}