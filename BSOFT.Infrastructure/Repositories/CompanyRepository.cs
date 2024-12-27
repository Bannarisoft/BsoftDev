using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Azure;


namespace BSOFT.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _imapper;

         public CompanyRepository(ApplicationDbContext applicationDbContext, IMapper imapper)
        {
            _applicationDbContext = applicationDbContext;
            _imapper = imapper;
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
                .FirstOrDefaultAsync(b => b.Id == id);
        }
           public async Task<int> UpdateAsync(int id, Company company)
        {
            Console.WriteLine("Hello Handler");
            Console.WriteLine(id);
            var existingCompany = await _applicationDbContext.Companies.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            
          
            
            if (existingCompany != null)
            {
                existingCompany.CompanyName = company.CompanyName;
                existingCompany.LegalName = company.LegalName;
                existingCompany.GstNumber = company.GstNumber;
                existingCompany.TIN = company.TIN;
                existingCompany.TAN = company.TAN;
                existingCompany.CSTNo = company.CSTNo;
                existingCompany.YearOfEstablishment = company.YearOfEstablishment;
                existingCompany.Website = company.Website;
                existingCompany.Logo = company.Logo;
                existingCompany.EntityId = company.EntityId;
                existingCompany.IsActive = company.IsActive;
                _applicationDbContext.Companies.Update(existingCompany);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }
         public async Task<int> DeleteAsync(int id,Company company)
        {
            
            var companyToDelete = await _applicationDbContext.Companies.FirstOrDefaultAsync(u => u.Id == id);
            if (companyToDelete != null)
            {
                companyToDelete.IsActive = company.IsActive;
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
                     Id = r.Id,
                     CompanyName = r.CompanyName
                 })
                 .ToListAsync();
        }
    }
}