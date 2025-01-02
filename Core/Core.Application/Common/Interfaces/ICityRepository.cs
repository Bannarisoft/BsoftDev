using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces
{
    public interface ICityRepository
    {
        Task<Cities> CreateAsync(Cities city);
        Task<int>  UpdateAsync(int cityId,Cities city);
        Task<int>  DeleteAsync(int cityId,Cities city);
        Task<Cities>  GetByIdAsync(int cityId);
        Task<List<Cities>> GetAllCityAsync();        
        Task<List<Cities>> GetByCityNameAsync(string cityName);        
        Task<bool> StateExistsAsync(int stateId); 
         Task<bool> GetCityByCodeAsync(string cityCode, int stateId); 
    }
}