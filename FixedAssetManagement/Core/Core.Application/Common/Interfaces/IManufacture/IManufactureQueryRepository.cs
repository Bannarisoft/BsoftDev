using Core.Application.Manufacture.Queries.GetManufacture;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IManufacture
{
    public interface IManufactureQueryRepository
    {
        Task<ManufactureDTO> GetByIdAsync(int Id);
        Task<(List<ManufactureDTO>, int)> GetAllManufactureAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<List<ManufactureDTO>> GetByManufactureNameAsync(string manufactureName);
        Task<List<Core.Domain.Entities.MiscMaster>> GetManufactureTypeAsync();
        Task<bool> CountrySoftDeleteValidation(int countryId);
        Task<bool> CitySoftDeleteValidation(int cityId);
        Task<bool> StateSoftDeleteValidation( int stateId);
    }
}