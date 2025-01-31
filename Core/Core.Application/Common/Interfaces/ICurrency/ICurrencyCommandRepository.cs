using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.ICurrency
{
    public interface ICurrencyCommandRepository
    {
      Task<int> CreateAsync(Core.Domain.Entities.Currency currency);
      Task<bool> ExistsByCodeAsync(string code); // Check if code exists
      Task<int> UpdateAsync(int Id,Core.Domain.Entities.Currency currency);
      Task<bool> SoftDeleteAsync(int id);
        
    }

}
