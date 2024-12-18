using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BSOFT.Application.Common.Interfaces;
using MediatR;
using System.Text;



namespace BSOFT.Application.Companies.Queries.GetCompanyAutoComplete
{
    public class GetCompanyAutoCompleteQueryHandler : IRequestHandler<GetCompanyAutoCompleteQuery,List<CompanyAutoCompleteVm>>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
         public GetCompanyAutoCompleteQueryHandler(ICompanyRepository companyRepository, IMapper mapper)
         {
             _companyRepository =companyRepository;
            _mapper =mapper;
         }  
          public async Task<List<CompanyAutoCompleteVm>> Handle(GetCompanyAutoCompleteQuery request, CancellationToken cancellationToken)
          {
                
       
            var company = await _companyRepository.GetCompany(request.SearchPattern);
            // Map to the application-specific DTO
            return company.Select(r => new CompanyAutoCompleteVm
            {
                CoId = r.CoId,
                CompanyName = r.CompanyName
            }).ToList();

         } 
    }
}