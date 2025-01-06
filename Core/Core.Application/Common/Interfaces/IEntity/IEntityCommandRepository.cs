using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.Application.Common.Interfaces.IEntity  
{
  using Entity = Core.Domain.Entities.Entity;
    public interface IEntityCommandRepository
    {
      
    
      Task<Entity> CreateAsync(Entity entity);
      Task<int> UpdateAsync(int Id,Entity entity);
      Task<int> DeleteAsync(int Id,Entity entity);
         



       
    }
}