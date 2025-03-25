using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IWorkCenter
{
    public interface IWorkCenterQueryRepository
    {
         Task<Core.Domain.Entities.WorkCenter?> GetByIdAsync(int Id);
         Task<(List<Core.Domain.Entities.WorkCenter>,int)> GetAllWorkCenterGroupAsync(int PageNumber, int PageSize, string? SearchTerm);
         Task<List<Core.Domain.Entities.WorkCenter>> GetWorkCenterGroups(string searchPattern);
    }
}