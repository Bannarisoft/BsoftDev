using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.ICountry
{
    public interface ICountryQueryRepository
    {
        Task<Countries> GetByIdAsync(int countryId);
        Task<List<Countries>> GetAllCountriesAsync();
        Task<List<Countries>> GetByCountryNameAsync(string countryName);        
    }
}