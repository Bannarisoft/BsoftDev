using AutoMapper;
using MediatR;
using Core.Application.Companies.Queries.GetCompanies;
using System.Data;
using Core.Application.Common.Interfaces.ICompany;
using Core.Domain.Entities;


namespace Core.Application.Companies.Queries.GetCompanyAutoComplete
{
    public class GetCompanyAutoCompleteQueryHandler : IRequestHandler<GetCompanyAutoCompleteQuery,List<CompanyAutoCompleteDTO>>
    { 
        private readonly ICompanyQueryRepository _companyRepository;
        private readonly IMapper _mapper;
         public GetCompanyAutoCompleteQueryHandler(ICompanyQueryRepository companyRepository, IMapper mapper)
         {
             _companyRepository = companyRepository;
             _mapper =mapper;
         }  
          public async Task<List<CompanyAutoCompleteDTO>> Handle(GetCompanyAutoCompleteQuery request, CancellationToken cancellationToken)
          {
           /*  var searchPattern = "%" + request.SearchPattern + "%";

                 const string query = @"
                 SELECT 
                Id, 
                CompanyName
            FROM AppData.Company where CompanyName like @SearchPattern";

            var company = await _dbConnection.QueryAsync<CompanyAutoCompleteDTO>(query, new { SearchPattern = searchPattern });
            // Map to the application-specific DTO
            // return company.Select(c => new CompanyAutoCompleteDTO
            // {
            //     CompanyId = c.Id,
            //     CompanyName = c.CompanyName
            // }).ToList();
            return company.AsList(); */

              var result = await _companyRepository.GetCompany(request.SearchPattern);
            //return _mapper.Map<List<DivisionDTO>>(result);
            return _mapper.Map<List<CompanyAutoCompleteDTO>>(result);            

         } 
    }
}