using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;
using Core.Application.Common.Interfaces.ICompany;
using Core.Application.Common.HttpResponse;
using Core.Domain.Events;

namespace Core.Application.Companies.Commands.DeleteCompany
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, ApiResponseDTO<bool>>
    {
        private readonly ICompanyCommandRepository _icompanyRepository;
        private readonly IMapper _imapper;
        private readonly IMediator _mediator;
         public DeleteCompanyCommandHandler(ICompanyCommandRepository companyRepository ,IMapper imapper, IMediator mediator)
        {
            _icompanyRepository = companyRepository;
            _imapper = imapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<bool>> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
            var company  = _imapper.Map<Company>(request.CompanyDelete);
            var result = await _icompanyRepository.DeleteAsync(request.Id,company);

            
            if (result)
            {
                 //Domain Event
                 var domainEvent = new AuditLogsDomainEvent(
                     actionDetail: "Delete",
                     actionCode: "",
                     actionName: "",
                     details: $"Company '{company.Id}' was deleted.",
                     module:"Company"
                 );
                 await _mediator.Publish(domainEvent, cancellationToken);
                return new ApiResponseDTO<bool>{IsSuccess = true, Message = "Company deleted successfully"};
            }
            return new ApiResponseDTO<bool>{IsSuccess = false, Message = "Company not deleted"};
        }
    }
}