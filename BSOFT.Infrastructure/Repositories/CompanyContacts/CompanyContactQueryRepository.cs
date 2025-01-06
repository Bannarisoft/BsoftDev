using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using Core.Application.Common.Interfaces.ICompanyContact;


namespace BSOFT.Infrastructure.Repositories.CompanyContacts
{
    public class CompanyContactQueryRepository : ICompanyContactQueryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CompanyContactQueryRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }


        public Task<List<CompanyContact>> GetAllCompaniesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CompanyContact> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }   
    }
}