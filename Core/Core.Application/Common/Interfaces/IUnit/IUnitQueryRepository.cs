using Core.Application.Units.Queries.GetUnits;
using Core.Domain.Entities;



namespace Core.Application.Common.Interfaces.IUnit
{
    public interface IUnitQueryRepository
    {
       Task<List<UnitDto>> GetAllUnitsAsync();
       Task<List<UnitDto>> GetByIdAsync(int Id);     
       Task<List<UnitDto>> GetUnit(string searchPattern);              
    }
    

   
}