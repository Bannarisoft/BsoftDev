using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using Core.Application.Companies.Queries.GetCompanies;
using System.Data;
using Dapper;

namespace Core.Application.Companies.Queries.GetCompanyById
{
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery,GetCompanyDTO>
    {
        private readonly IDbConnection _dbConnection;
        private readonly IMapper _mapper;

         public GetCompanyByIdQueryHandler(IDbConnection dbConnection, IMapper mapper)
        {
            _dbConnection = dbConnection;
            _mapper =mapper;
        } 
        public async Task<GetCompanyDTO> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
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
            LEFT JOIN AppData.CompanyContact B ON B.CompanyId = C.Id WHERE C.Id = @CompanyId";
           
             var company = await _dbConnection.QueryFirstOrDefaultAsync<GetCompanyDTO>(query, new { CompanyId = request.CompanyId });
            return _mapper.Map<GetCompanyDTO>(company);
        }
    }
}