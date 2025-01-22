using AutoMapper;
using MediatR;
using Core.Application.Companies.Queries.GetCompanies;
using System.Data;
using Core.Application.Common.Interfaces.ICompany;
using Core.Domain.Entities;
using Core.Application.Common.HttpResponse;
using Core.Domain.Events;


namespace Core.Application.Companies.Queries.GetCompanyAutoComplete
{
    public class GetCompanyAutoCompleteQueryHandler : IRequestHandler<GetCompanyAutoCompleteQuery,ApiResponseDTO<List<CompanyAutoCompleteDTO>>>
    { 
        private readonly ICompanyQueryRepository _companyRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
         public GetCompanyAutoCompleteQueryHandler(ICompanyQueryRepository companyRepository, IMapper mapper, IMediator mediator)
         {
             _companyRepository = companyRepository;
             _mapper =mapper;
             _mediator = mediator;
         }  
          public async Task<ApiResponseDTO<List<CompanyAutoCompleteDTO>>> Handle(GetCompanyAutoCompleteQuery request, CancellationToken cancellationToken)
          {
         

              var result = await _companyRepository.GetCompany(request.SearchPattern);
              var company = _mapper.Map<List<CompanyAutoCompleteDTO>>(result);
              //Domain Event
                 var domainEvent = new AuditLogsDomainEvent(
                     actionDetail: "GetCompanyAutoComplete",
                     actionCode: "",
                     actionName: "",
                     details: $"Company details was fetched.",
                     module:"Company"
                 );
                 await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<CompanyAutoCompleteDTO>> { IsSuccess = true, Message = "Success", Data = company };            

         } 
    }
}