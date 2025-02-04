using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.ICurrency
{
    public interface ICurrencyQueryRepository
    {
      Task<List<Core.Domain.Entities.Currency>> GetAllCurrencyAsync();
      Task<List<Core.Domain.Entities.Currency>> GetByIdAsync(int Id);
      Task<List<Core.Domain.Entities.Currency>> GetByCurrencyNameAsync(string currency);
    }
}