using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.ICity
{
    public interface ICityCommandRepository
    {
        Task<Cities> CreateAsync(Cities city);
        Task<int>  UpdateAsync(int cityId,Cities city);
        Task<int>  DeleteAsync(int cityId,Cities city);    
        Task<bool> StateExistsAsync(int stateId); 
        Task<bool> GetCityByCodeAsync(string cityCode, int stateId); 
    }
}