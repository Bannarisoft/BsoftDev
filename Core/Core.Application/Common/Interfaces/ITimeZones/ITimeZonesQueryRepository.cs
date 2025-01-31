using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.ITimeZones
{
    public interface ITimeZonesQueryRepository
    {
      Task<List<Core.Domain.Entities.TimeZones>> GetAllTimeZonesAsync();
      Task<List<Core.Domain.Entities.TimeZones>> GetByIdAsync(int Id);
      Task<List<Core.Domain.Entities.TimeZones>> GetByTimeZonesNameAsync(string timeZones);
    }
}