using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IPreventiveScheduler
{
    public interface IPreventiveSchedulerQuery
    {
        Task<(IEnumerable<dynamic> PreventiveSchedulerList,int)> GetAllPreventiveSchedulerAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<PreventiveSchedulerHdr> GetByIdAsync(int id);
        Task<List<PreventiveSchedulerHdr>> GetPreventiveScheduler(string searchPattern);
        Task<bool> SoftDeleteValidation(int Id); 
        Task<bool> AlreadyExistsAsync(string ShiftName,int? id = null);
        Task<bool> NotFoundAsync(int id );
    }
}