using AutoMapper;
using MediatR;
using Core.Application.Companies.Queries.GetCompanies;
using System.Data;
using Core.Application.Common.Interfaces.ICompany;
using Core.Domain.Entities;
using Core.Application.Common.HttpResponse;


namespace Core.Application.Companies.Queries.GetCompanyAutoComplete
{
    public class GetCompanyAutoCompleteQueryHandler : IRequestHandler<GetCompanyAutoCompleteQuery,ApiResponseDTO<List<CompanyAutoCompleteDTO>>>
    { 
        private readonly ICompanyQueryRepository _companyRepository;
        private readonly IMapper _mapper;
         public GetCompanyAutoCompleteQueryHandler(ICompanyQueryRepository companyRepository, IMapper mapper)
         {
             _companyRepository = companyRepository;
             _mapper =mapper;
         }  
          public async Task<ApiResponseDTO<List<CompanyAutoCompleteDTO>>> Handle(GetCompanyAutoCompleteQuery request, CancellationToken cancellationToken)
          {
         

              var result = await _companyRepository.GetCompany(request.SearchPattern);
              var company = _mapper.Map<List<CompanyAutoCompleteDTO>>(result);
            return new ApiResponseDTO<List<CompanyAutoCompleteDTO>> { IsSuccess = true, Message = "Success", Data = company };            

         } 
    }
}