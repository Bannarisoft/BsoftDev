using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using Core.Application.Common.Interfaces.ICompany;
using Core.Application.Common.Interfaces;

namespace BSOFT.Infrastructure.Repositories.Companies
{
    public class CompanyCommandRepository : ICompanyCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _imapper;

         public CompanyCommandRepository(ApplicationDbContext applicationDbContext, IMapper imapper)
        {
            _applicationDbContext = applicationDbContext;
            _imapper = imapper;
        }

      
         public async Task<int> CreateAsync(Company company)
        {
            var entry =_applicationDbContext.Entry(company);
            await _applicationDbContext.Companies.AddAsync(company);
            await _applicationDbContext.SaveChangesAsync();
            return company.Id;
        }      
           public async Task<bool> UpdateAsync(int id, Company company)
        {
            var existingCompany = await _applicationDbContext.Companies
            .Include(c => c.CompanyAddress)
            .Include(c => c.CompanyContact)
            .AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            
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
                existingCompany.CompanyAddress.AddressLine1 = company.CompanyAddress.AddressLine1;
                existingCompany.CompanyAddress.AddressLine2 = company.CompanyAddress.AddressLine2;
                existingCompany.CompanyAddress.PinCode = company.CompanyAddress.PinCode;
                existingCompany.CompanyAddress.AlternatePhone = company.CompanyAddress.AlternatePhone;
                existingCompany.CompanyAddress.Phone = company.CompanyAddress.Phone;
                existingCompany.CompanyAddress.CityId = company.CompanyAddress.CityId;
                existingCompany.CompanyAddress.StateId = company.CompanyAddress.StateId;
                existingCompany.CompanyContact.Designation = company.CompanyContact.Designation;
                existingCompany.CompanyContact.Email = company.CompanyContact.Email;
                existingCompany.CompanyContact.Name = company.CompanyContact.Name;
                existingCompany.CompanyContact.Phone = company.CompanyContact.Phone;
                existingCompany.CompanyContact.Remarks = company.CompanyContact.Remarks;
                _applicationDbContext.Companies.Update(existingCompany);
                return await _applicationDbContext.SaveChangesAsync() >0;
            }
            return false; 
        }
         public async Task<int> DeleteAsync(int id,Company company)
        {
            Console.WriteLine("Hello Handler");
            Console.WriteLine(id);
            Console.WriteLine(company.IsActive);
            var companyToDelete = await _applicationDbContext.Companies.FirstOrDefaultAsync(u => u.Id == id);
            if (companyToDelete != null)
            {
                companyToDelete.IsActive = company.IsActive;
                return await _applicationDbContext.SaveChangesAsync();
            }
            Console.WriteLine(companyToDelete.Id);
            return 0; // No user found
        }     
    }
}