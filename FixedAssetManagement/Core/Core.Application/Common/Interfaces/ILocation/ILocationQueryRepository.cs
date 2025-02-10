using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.ILocation
{
    public interface ILocationQueryRepository
    {
        Task<(List<Core.Domain.Entities.Location>,int)> GetAllLocationAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<Core.Domain.Entities.Location> GetByIdAsync(int id);
        Task<List<Core.Domain.Entities.Location>> GetLocation(string searchPattern, int companyId);
        Task<Core.Domain.Entities.Location?> GetByLocationNameAsync(string name,int? id = null);
    }
}