using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Core.Application.Common.Interfaces.IPreventiveSchedulerLog
{
    public interface IPreventiveScheduleLogService
    {
        Task<bool> CaptureLogs(int? PreventiveScheduleId,int? PreventiveScheduleDetailId,string ActionType,string ChangedFields);
        
    }
}