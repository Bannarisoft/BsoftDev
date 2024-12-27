using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interface
{
    public interface ICountryRepository
    {
        Task<Countries> CreateAsync(Countries country);        
        Task<int>  UpdateAsync(int countryId,Countries country);
        Task<int>  DeleteAsync(int countryId,Countries country);
        Task<Countries> GetByIdAsync(int countryId);
        Task<List<Countries>> GetAllCountriesAsync();
        Task<List<Countries>> GetByCountryNameAsync(string countryname);
    }
}