using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

         public CompanyRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

         public async Task<List<Company>> GetAllCompaniesAsync()
        {
            return await _applicationDbContext.Companies.ToListAsync();
        }
         public async Task<Company> CreateAsync(Company company)
        {
            await _applicationDbContext.Companies.AddAsync(company);
            await _applicationDbContext.SaveChangesAsync();
            return company;
        }
         public async Task<Company> GetByIdAsync(int id)
        {
            
            return await _applicationDbContext.Companies.AsNoTracking()
                .FirstOrDefaultAsync(b => b.CoId == id);
        }
           public async Task<int> UpdateAsync(int id, Company company)
        {
            var existingCompany = await _applicationDbContext.Companies.FirstOrDefaultAsync(u => u.CoId == id);
            if (existingCompany != null)
            {
                existingCompany.CompanyName = company.CompanyName;
                existingCompany.LegalName = company.LegalName;
                existingCompany.Address1 = company.Address1;
                existingCompany.Address2 = company.Address2;
                existingCompany.Address3 = company.Address3;
                existingCompany.Phone = company.Phone;
                existingCompany.Email = company.Email;
                existingCompany.GstNumber = company.GstNumber;
                existingCompany.TIN = company.TIN;
                existingCompany.TAN = company.TAN;
                existingCompany.CSTNo = company.CSTNo;
                existingCompany.YearofEstablishment = company.YearofEstablishment;
                existingCompany.Website = company.Website;
                existingCompany.EntityId = company.EntityId;
                existingCompany.IsActive = company.IsActive;

                _applicationDbContext.Companies.Update(existingCompany);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }
         public async Task<int> DeleteAsync(int id)
        {
            var companyToDelete = await _applicationDbContext.Companies.FirstOrDefaultAsync(u => u.CoId == id);
            if (companyToDelete != null)
            {
                _applicationDbContext.Companies.Remove(companyToDelete);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }
         public async Task<List<Company>>  GetCompany(string searchPattern = null)
        {
                       return await _applicationDbContext.Companies
                 .Where(r => EF.Functions.Like(r.CompanyName, $"%{searchPattern}%")) 
                 .Select(r => new Company
                 {
                     CoId = r.CoId,
                     CompanyName = r.CompanyName
                 })
                 .ToListAsync();
        }
    }
}