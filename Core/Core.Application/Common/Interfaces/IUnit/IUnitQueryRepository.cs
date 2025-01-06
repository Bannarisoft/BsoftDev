using Core.Domain.Entities;



namespace Core.Application.Common.Interfaces.IUnit
{
    public interface IUnitQueryRepository
    {
       Task<List<Unit>> GetAllUnitsAsync();
       Task<Unit> GetByIdAsync(int Id);     
       Task<List<Unit>> GetUnit(string searchPattern);

       


       
    }
    

   
}