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
        private readonly ICompanyRepository _icompanyRepository;
        private readonly ICompanyAddressRepository _icompanyAddressRepository;
        private readonly ICompanyContactRepository _icompanyContactRepository;
        private readonly IMapper _imapper;
         public GetCompanyAutoCompleteQueryHandler(ICompanyRepository icompanyRepository,ICompanyAddressRepository icompanyAddressRepository,ICompanyContactRepository icompanyContactRepository, IMapper imapper)
         {
             _icompanyRepository =icompanyRepository;
             _icompanyAddressRepository =icompanyAddressRepository;
             _icompanyContactRepository =icompanyContactRepository;
            _imapper =imapper;
         }  
          public async Task<List<CompanyAutoCompleteVm>> Handle(GetCompanyAutoCompleteQuery request, CancellationToken cancellationToken)
          {
                
       
            var company = await _icompanyRepository.GetCompany(request.SearchPattern);
            // Map to the application-specific DTO
            return company.Select(c => new CompanyAutoCompleteVm
            {
                CompanyId = c.Id,
                CompanyName = c.CompanyName
            }).ToList();

         } 
    }
}