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
using Core.Application.Common.Interfaces.ICompany;
using Core.Domain.Entities;
using Core.Application.Common.HttpResponse;
using Core.Domain.Events;

namespace Core.Application.Companies.Queries.GetCompanyById
{
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery,ApiResponseDTO<GetCompanyDTO>>
    {
          private readonly ICompanyQueryRepository _companyRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
         public GetCompanyByIdQueryHandler(ICompanyQueryRepository companyRepository, IMapper mapper, IMediator mediator)
        {
              _companyRepository = companyRepository;
             _mapper =mapper;
             _mediator = mediator;
        } 
        public async Task<ApiResponseDTO<GetCompanyDTO>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
           
            var result = await _companyRepository.GetByIdAsync(request.CompanyId);
            var company = _mapper.Map<GetCompanyDTO>(result);
             //Domain Event
                 var domainEvent = new AuditLogsDomainEvent(
                     actionDetail: "GetCompanyById",
                     actionCode: "",
                     actionName: "",
                     details: $"Company details {company.Id} was fetched.",
                     module:"Company"
                 );
                 await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<GetCompanyDTO> { IsSuccess = true, Message = "Success", Data = company };
        }
    }
}