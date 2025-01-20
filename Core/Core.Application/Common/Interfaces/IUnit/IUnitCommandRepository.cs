using Core.Domain.Entities;



namespace Core.Application.Common.Interfaces.IUnit
{
    public interface IUnitCommandRepository
    {      
       Task<int> CreateUnitAsync(Unit unit);
       Task<UnitAddress> CreateUnitAddressAsync(UnitAddress unitAddress);
       Task<UnitContacts> CreateUnitContactsAsync(UnitContacts unitContacts);
       
       Task<int> UpdateUnitAsync(int Id, Unit unit);
       Task UpdateUnitAddressAsync(int Id, UnitAddress unitAddress);
       Task UpdateUnitContactsAsync(int Id, UnitContacts unitContacts);
       Task<int> DeleteUnitAsync(int Id, Unit unit);
       
    }
    

   
}