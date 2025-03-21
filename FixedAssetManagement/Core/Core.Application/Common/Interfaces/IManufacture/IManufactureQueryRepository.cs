using Core.Application.Manufacture.Queries.GetManufacture;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IManufacture
{
    public interface IManufactureQueryRepository
    {
        Task<Manufactures>  GetByIdAsync(int Id);
        Task<(List<ManufactureDTO>,int)> GetAllManufactureAsync(int PageNumber, int PageSize, string? SearchTerm);        
        Task<List<ManufactureDTO>> GetByManufactureNameAsync(string manufactureName); 
        Task<List<Core.Domain.Entities.MiscMaster>> GetManufactureTypeAsync();    
    }
}