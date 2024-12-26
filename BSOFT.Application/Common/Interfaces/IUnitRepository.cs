using BSOFT.Domain.Entities;



namespace BSOFT.Application.Common.Interfaces
{
    public interface IUnitRepository
    {
       Task<List<Unit>> GetAllUnitsAsync();
       Task<Unit> GetByIdAsync(int Id);
       Task<int> CreateUnitAsync(Unit unit);
       Task<UnitAddress> CreateUnitAddressAsync(UnitAddress unitAddress);
       Task<UnitContacts> CreateUnitContactsAsync(UnitContacts unitContacts);
       
       Task UpdateUnitAsync(int Id, Unit unit);
       Task UpdateUnitAddressAsync(int Id, UnitAddress unitAddress);
       Task UpdateUnitContactsAsync(int Id, UnitContacts unitContacts);
       Task DeleteUnitAsync(int Id, Unit unit);
       Task<List<Unit>> GetUnit(string searchPattern);

       


       
    }
    

   
}