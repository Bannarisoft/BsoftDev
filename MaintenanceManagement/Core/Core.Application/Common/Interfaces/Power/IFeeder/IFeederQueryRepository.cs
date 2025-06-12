using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.Power;

namespace Core.Application.Common.Interfaces.Power.IFeeder
{
    public interface IFeederQueryRepository
    {
        Task<(List<Feeder>, int)> GetAllFeederAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<Feeder> GetFeederByIdAsync(int id);
        Task<bool> AlreadyExistsAsync(string feederCode, int? id = null);
        Task<bool> NotFoundAsync(int id);

        Task<List<Feeder>> GetFeederAutoComplete(string searchPattern);
          
        
    }
}