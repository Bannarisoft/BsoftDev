using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Core.Application.Common.Interfaces.ICompanyContact;


namespace BSOFT.Infrastructure.Repositories.CompanyContacts
{
    public class CompanyContactCommandRepository : ICompanyContactCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CompanyContactCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<CompanyContact> CreateAsync(CompanyContact companyContact)
        {
            await _applicationDbContext.CompanyContacts.AddAsync(companyContact);
            await _applicationDbContext.SaveChangesAsync();
            return companyContact;
        }

        public Task<int> DeleteAsync(int id, CompanyContact companyContact)
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdateAsync(int id, CompanyContact companyContact)
        {
            var existingCompany = await _applicationDbContext.CompanyContacts
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>  u.CompanyId == companyContact.CompanyId);
            
            if (existingCompany != null)
            {
                existingCompany.Name = companyContact.Name;
                existingCompany.Designation = companyContact.Designation;
                existingCompany.Email = companyContact.Email;
                existingCompany.Phone = companyContact.Phone;
                existingCompany.Remarks = companyContact.Remarks;

                _applicationDbContext.CompanyContacts.Update(existingCompany);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0;
        }
    }
}