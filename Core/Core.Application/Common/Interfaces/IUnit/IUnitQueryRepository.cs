using Core.Application.Units.Queries.GetUnits;
using Core.Domain.Entities;



namespace Core.Application.Common.Interfaces.IUnit
{
    public interface IUnitQueryRepository
    {
       Task<List<Unit>> GetAllUnitsAsync();
       Task<List<Unit>> GetByIdAsync(int Id);     
       Task<List<Unit>> GetUnit(string searchPattern);              
    }
    
   
}