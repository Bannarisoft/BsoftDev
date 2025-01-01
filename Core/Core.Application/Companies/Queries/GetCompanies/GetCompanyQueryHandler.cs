using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using System.Data;
using Dapper;

namespace Core.Application.Companies.Queries.GetCompanies
{
    public class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery,List<GetCompanyDTO>>
    {
        private readonly IDbConnection _dbConnection;
        // private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        public GetCompanyQueryHandler(IDbConnection dbConnection, IMapper mapper)
        {
           _dbConnection = dbConnection;
            _mapper =mapper;
        } 
        public async Task<List<GetCompanyDTO>> Handle(GetCompanyQuery requst, CancellationToken cancellationToken){

        const string query = @"
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

            return companies.AsList();
        }
    }
}