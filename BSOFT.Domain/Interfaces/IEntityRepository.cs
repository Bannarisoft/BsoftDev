using BSOFT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BSOFT.Domain.Interfaces
{
    public interface IEntityRepository
    {
       Task<List<Entity>> GetAllEntityAsync();
       Task<Entity> GetByIdAsync(int Id);
       Task<string> GenerateEntityCodeAsync();
    
      Task<Entity> CreateAsync(Entity entity);
      Task<int> UpdateAsync(int Id,Entity entity);
      Task<int> DeleteAsync(int Id,Entity entity);
      



       
    }
}