using AutoMapper;
using MediatR;
using Core.Application.Common.Interfaces.ICompany;

namespace Core.Application.Companies.Queries.GetCompanies
{
    public class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery,List<GetCompanyDTO>>
    {  
        private readonly ICompanyQueryRepository _companyRepository;
        private readonly IMapper _mapper;
        public GetCompanyQueryHandler(ICompanyQueryRepository companyRepository, IMapper mapper)
        {
             _companyRepository = companyRepository;
             _mapper =mapper;
        } 
        public async Task<List<GetCompanyDTO>> Handle(GetCompanyQuery requst, CancellationToken cancellationToken){

        /* const string query = @"
            SELECT 
                C.Id, 
                C.CompanyName, 
                C.LegalName,
                GstNumber,
                TIN,
                TAN,
                CSTNo,
                YearOfEstablishment,
                Website,
                Logo,
                EntityId, 
                IsActive,
                AddressLine1,
                AddressLine2,
                PinCode,
                CountryId,
                StateId,
                CityId,
                A.Phone AS AddressPhone, 
                Name,
                Designation,
                Email,
                B.Phone AS ContactPhone,
                Remark
            FROM AppData.Company C
            LEFT JOIN AppData.CompanyAddress A ON A.CompanyId = C.Id
            LEFT JOIN AppData.CompanyContact B ON B.CompanyId = C.Id";
            var companies = await _dbConnection.QueryAsync<GetCompanyDTO>(query);
            // var companies = await _companyRepository.GetAllCompaniesAsync();
            
           // var companylist = _mapper.Map<List<CompanyDTO>>(companies);

            return companies.AsList(); */

            var companies = await _companyRepository.GetAllCompaniesAsync();             
            var companylist = _mapper.Map<List<GetCompanyDTO>>(companies);
            return companylist.ToList();
        }
    }
}