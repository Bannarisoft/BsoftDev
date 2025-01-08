using Core.Application.Units.Queries.GetUnits;
using Core.Domain.Entities;



namespace Core.Application.Common.Interfaces.IUnit
{
    public interface IUnitQueryRepository
    {
       Task<List<Unit>> GetAllUnitsAsync();
       Task<Unit> GetByIdAsync(int Id);     
       Task<List<UnitDto>> GetUnit(string searchPattern);

       


       
    }
    

   
}