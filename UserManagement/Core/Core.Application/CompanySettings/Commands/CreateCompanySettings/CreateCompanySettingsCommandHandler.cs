using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICompanySettings;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.CompanySettings.Commands.CreateCompanySettings
{
    public class CreateCompanySettingsCommandHandler : IRequestHandler<CreateCompanySettingsCommand, ApiResponseDTO<int>>
    {
        private readonly ICompanyCommandSettings _icompanyCommandSettings;
        private readonly IMediator _mediator;
        private readonly IMapper _imapper;
        public CreateCompanySettingsCommandHandler(ICompanyCommandSettings icompanyCommandSettings, IMediator mediator, IMapper imapper)
        {
            _icompanyCommandSettings = icompanyCommandSettings;   
            _mediator = mediator;
            _imapper = imapper;
        }
        public async Task<ApiResponseDTO<int>> Handle(CreateCompanySettingsCommand request, CancellationToken cancellationToken)
        {
            var companySettings = _imapper.Map<Core.Domain.Entities.CompanySettings>(request);
            
            
            var CompanySettingsId =  await _icompanyCommandSettings.CreateAsync(companySettings);
            if (CompanySettingsId > 0)
            {
                     var domainEvent = new AuditLogsDomainEvent(
                     actionDetail: "Create",
                     actionCode: "",
                     actionName: "",
                     details: $"Company Setting '{CompanySettingsId}' was created.",
                     module:"Company Setting"
                 );
                 await _mediator.Publish(domainEvent, cancellationToken);
                 
                return new ApiResponseDTO<int>{IsSuccess = true, Message = "Company Settings created successfully", Data = CompanySettingsId};
            }
            return new ApiResponseDTO<int>{IsSuccess = false, Message = "Company Settings not created"};
        }
    }
}