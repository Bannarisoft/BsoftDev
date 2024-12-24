using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Infrastructure.Data;
using BSOFT.Application.Common.Interfaces;
using BSOFT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;


namespace BSOFT.Infrastructure.Repositories
{
    public class CompanyContactRepository : ICompanyContactRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CompanyContactRepository(ApplicationDbContext applicationDbContext)
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

        public Task<List<CompanyContact>> GetAllCompaniesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CompanyContact> GetByIdAsync(int id)
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