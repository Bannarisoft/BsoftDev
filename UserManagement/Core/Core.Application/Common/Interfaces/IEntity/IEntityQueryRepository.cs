using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.Application.Common.Interfaces.IEntity
{
  using Entity = Core.Domain.Entities.Entity;
  public interface IEntityQueryRepository
    {
      
      Task<(List<Entity>,int)> GetAllEntityAsync(int PageNumber, int PageSize, string? SearchTerm);
      Task<Entity> GetByIdAsync(int Id);
      Task<List<Entity>> GetByEntityNameAsync(string entity);
      Task<string> GenerateEntityCodeAsync();
      Task<bool> SoftDeleteValidation(int Id);
       
    }
}