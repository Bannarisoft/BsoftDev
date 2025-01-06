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
    public class CompanyQueryRepository : ICompanyQueryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _imapper;

         public CompanyQueryRepository(ApplicationDbContext applicationDbContext, IMapper imapper)
        {
            _applicationDbContext = applicationDbContext;
            _imapper = imapper;
        }

         public async Task<List<Company>> GetAllCompaniesAsync()
        {
            return await _applicationDbContext.Companies.ToListAsync();
        }
         public async Task<Company> GetByIdAsync(int id)
        {            
            return await _applicationDbContext.Companies.AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
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