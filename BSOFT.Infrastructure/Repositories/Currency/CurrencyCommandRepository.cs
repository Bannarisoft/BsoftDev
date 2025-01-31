using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Interfaces.ICurrency;
using Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BSOFT.Infrastructure.Repositories.Currency
{
    public class CurrencyCommandRepository : ICurrencyCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CurrencyCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<int> CreateAsync(Core.Domain.Entities.Currency currency)
        {
        // Add the Currency to the DbContext
        await _applicationDbContext.Currency.AddAsync(currency);

        // Save changes to the database
        await _applicationDbContext.SaveChangesAsync();

        // Return the ID of the created currency
        return currency.Id;
    }
      public async Task<int> UpdateAsync(int id, Core.Domain.Entities.Currency  currency)
    {   
    
        var existingcurrency = await _applicationDbContext.Currency.FirstOrDefaultAsync(u => u.Id == id);

        // If the currency does not exist, throw a CustomException
        if (existingcurrency == null)
        {
            return -1; //indicate failure
        }

        // Update the existing currency's properties
        existingcurrency.Code = currency.Code;
        existingcurrency.Name = currency.Name;
        existingcurrency.IsActive = currency.IsActive;
  


        // Mark the currency as modified
        _applicationDbContext.Currency.Update(existingcurrency);

        // Save changes to the database
        await _applicationDbContext.SaveChangesAsync();

        return 1; // Indicate success
    
}


    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _applicationDbContext.Currency.AnyAsync(c => c.Code == code);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var currency = await _applicationDbContext.Currency.FirstOrDefaultAsync(c => c.Id == id);
        if (currency == null)
        return false;
        currency.IsDeleted = CurrencyEnum.CurrencyDelete.Deleted; // Mark as Deleted
        _applicationDbContext.Currency.Update(currency);
        await _applicationDbContext.SaveChangesAsync();
        return true;
    }

    }
}