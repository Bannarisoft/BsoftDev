using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.ISubLocation
{
    public interface ISubLocationQueryRepository
    {
        Task<(List<Core.Domain.Entities.SubLocation>,int)> GetAllSubLocationAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<Core.Domain.Entities.SubLocation> GetByIdAsync(int id);
        Task<List<Core.Domain.Entities.SubLocation>> GetSubLocation(string searchPattern);
        Task<Core.Domain.Entities.SubLocation?> GetBySubLocationNameAsync(string name,int DepartmentId,int? id = null);
    }
}