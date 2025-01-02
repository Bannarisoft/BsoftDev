using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interface
{
    public interface IStateRepository
    {
        Task<States> CreateAsync(States state);        
        Task<int>  UpdateAsync(int stateId,States state);
        Task<int>  DeleteAsync(int stateId,States state);
        Task<States>  GetByIdAsync(int stateId);
        Task<List<States>> GetAllStatesAsync();        
        Task<List<States>> GetByStateNameAsync(string stateName);           
        Task<bool> CountryExistsAsync(int stateId); 
         Task<bool> GetStateByCodeAsync(string StateCode, int countryId); 
    }
}