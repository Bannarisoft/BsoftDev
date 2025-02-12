using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IManufacture
{
    public interface IManufactureQueryRepository
    {
        Task<Manufactures>  GetByIdAsync(int Id);
        Task<(List<Manufactures>,int)> GetAllManufactureAsync(int PageNumber, int PageSize, string? SearchTerm);        
        Task<List<Manufactures>> GetByManufactureNameAsync(string manufactureName);  
    }
}