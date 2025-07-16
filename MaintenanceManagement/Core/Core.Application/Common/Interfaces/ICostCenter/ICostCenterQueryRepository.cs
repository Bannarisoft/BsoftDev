using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.ICostCenter
{
    public interface ICostCenterQueryRepository
    {
        Task<Core.Domain.Entities.CostCenter?> GetByIdAsync(int Id);
        Task<(List<Core.Domain.Entities.CostCenter>, int)> GetAllCostCenterGroupAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<List<Core.Domain.Entities.CostCenter>> GetCostCenterGroups(string searchPattern);
        Task<bool> SoftDeleteValidation(int Id);          
        Task<bool> DepartmentSoftDeleteValidation(int Id);
    }
}