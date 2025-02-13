using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IManufacture
{
    public interface IManufactureCommandRepository
    {
        Task<Manufactures> CreateAsync(Manufactures manufacture);
        Task<int>  UpdateAsync(int Id,Manufactures manufacture);
        Task<int>  DeleteAsync(int Id,Manufactures manufacture);            
        Task<bool> ExistsByCodeAsync(string code );        
    }    
}