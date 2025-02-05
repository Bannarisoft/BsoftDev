using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ICompany;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Companies.Commands.UpdateCompany
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, ApiResponseDTO<bool>>
    {
        private readonly ICompanyCommandRepository _icompanyRepository;
        private readonly IFileUploadService _ifileUploadService;
        private readonly IMapper _imapper;
        private readonly ICompanyQueryRepository _companyQueryRepository;
        private readonly IMediator _mediator;

        public UpdateCompanyCommandHandler(ICompanyCommandRepository icompanyRepository, IMapper imapper, IFileUploadService ifileUploadService, ICompanyQueryRepository companyQueryRepository, IMediator mediator)
        {
            _icompanyRepository = icompanyRepository;
            _imapper = imapper;
            _ifileUploadService = ifileUploadService;
            _companyQueryRepository = companyQueryRepository;
            _mediator = mediator;
        }

          public async Task<ApiResponseDTO<bool>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            
            
            var existingCompany = await _companyQueryRepository.GetByCompanynameAsync(request.Company.CompanyName,request.Company.Id);

              if (existingCompany != null)
              {
                  return new ApiResponseDTO<bool>{IsSuccess = false, Message = "Company already exists"};
              }
            var company  = _imapper.Map<Company>(request.Company);
            
            var  CompanyId = await _icompanyRepository.UpdateAsync(request.Company.Id, company);
           
           if (CompanyId)
           {
             
                 var domainEvent = new AuditLogsDomainEvent(
                     actionDetail: "Update",
                     actionCode: "",
                     actionName: "",
                     details: $"Company '{company.Id}' was updated.",
                     module:"Company"
                 );
                 await _mediator.Publish(domainEvent, cancellationToken);
               return new ApiResponseDTO<bool>{IsSuccess = true, Message = "Company updated successfully"};
           }
            return new ApiResponseDTO<bool>{IsSuccess = false, Message = "Company not updated"};
        }
    }
}