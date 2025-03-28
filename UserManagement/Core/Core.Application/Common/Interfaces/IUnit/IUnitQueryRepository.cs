using Core.Application.Units.Queries.GetUnits;
using Core.Domain.Entities;



namespace Core.Application.Common.Interfaces.IUnit
{
    public interface IUnitQueryRepository
    {
       Task<(List<Unit>,int)> GetAllUnitsAsync(int PageNumber, int PageSize, string? SearchTerm);
       Task<Unit> GetByIdAsync(int Id);     
       Task<List<Unit>> GetUnit(string searchPattern,int userId,int CompanyId); 
       Task<bool> FKColumnExistValidation(int Id);            
    }
    
   
}