using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.ICompanyAddress;

namespace BSOFT.Infrastructure.Repositories.CompanyAddresses
{
    public class CompanyAddressQueryRepository : ICompanyAddressQueryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext; 
        public CompanyAddressQueryRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }


        public Task<List<CompanyAddress>> GetAllCompaniesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CompanyAddress> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }     
    }
}