using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.ICompanyAddress;

namespace BSOFT.Infrastructure.Repositories.CompanyAddresses
{
    public class CompanyAddressCommandRepository : ICompanyAddressCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext; 
        public CompanyAddressCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<CompanyAddress> CreateAsync(CompanyAddress companyAddress)
        {
            await _applicationDbContext.companyAddresses.AddAsync(companyAddress);
            await _applicationDbContext.SaveChangesAsync();
            return companyAddress;
        }

        public Task<int> DeleteAsync(int id, CompanyAddress companyAddress)
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdateAsync(int id, CompanyAddress companyAddress)
        {
              var existingCompany = await _applicationDbContext.companyAddresses
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.CompanyId == companyAddress.CompanyId);
            if (existingCompany != null)
            {
                existingCompany.AddressLine1 = companyAddress.AddressLine1;
                existingCompany.AddressLine2 = companyAddress.AddressLine2;
                existingCompany.PinCode = companyAddress.PinCode;
                existingCompany.CountryId = companyAddress.CountryId;
                existingCompany.StateId = companyAddress.StateId;
                existingCompany.CityId = companyAddress.CityId;
                existingCompany.Phone = companyAddress.Phone;
                existingCompany.AlternatePhone = companyAddress.AlternatePhone;

                _applicationDbContext.companyAddresses.Update(existingCompany);
                
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0;
        }
    }
}