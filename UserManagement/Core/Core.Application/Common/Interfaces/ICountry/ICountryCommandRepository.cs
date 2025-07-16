using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.ICountry
{
    public interface ICountryCommandRepository
    {
        Task<Countries> CreateAsync(Countries country);        
        Task<int>  UpdateAsync(int countryId,Countries country);
        Task<int>  DeleteAsync(int countryId,Countries country);
        Task<Countries> GetCountryByCodeAsync(string countryName,string countryCode);              
    }
}