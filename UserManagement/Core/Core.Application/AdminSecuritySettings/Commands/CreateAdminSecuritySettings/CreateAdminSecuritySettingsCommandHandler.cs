using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettings;
using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAdminSecuritySettings;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.AdminSecuritySettings.Commands.CreateAdminSecuritySettings
{
    public class CreateAdminSecuritySettingsCommandHandler  :IRequestHandler<CreateAdminSecuritySettingsCommand, ApiResponseDTO<int>>
    {
        private readonly IAdminSecuritySettingsCommandRepository _adminSecuritySettingsCommandRepository;
        private readonly IMapper _mapper;
             
          private readonly IMediator _mediator; 
          private readonly ILogger<CreateAdminSecuritySettingsCommandHandler> _logger;

          public CreateAdminSecuritySettingsCommandHandler(IAdminSecuritySettingsCommandRepository adminSecuritySettingsCommandRepository,IMapper mapper,IMediator mediator,   ILogger<CreateAdminSecuritySettingsCommandHandler> logger)
        {
             _adminSecuritySettingsCommandRepository=adminSecuritySettingsCommandRepository;

            _mapper=mapper;
            _mediator=mediator;
            _logger=logger;
        }

        public async Task<ApiResponseDTO<int>> Handle(CreateAdminSecuritySettingsCommand request, CancellationToken cancellationToken)
        {        
             _logger.LogInformation("Processing CreateAdminSecuritySettingsCommand request.");

              var adminsettingsEntity = _mapper.Map<Core.Domain.Entities.AdminSecuritySettings>(request);

              
                _logger.LogDebug($"Mapped CreateAdminSecuritySettingsCommand to AdminSecuritySettings entity {request}.");

                var result = await _adminSecuritySettingsCommandRepository.CreateAsync(adminsettingsEntity);
                _logger.LogInformation($"Successfully created AdminSecuritySettings entity with ID: { result.Id}.");

                // Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Create",
                    actionCode: result.Id.ToString(),
                    actionName: "",
                    details: $"New Admin settings was created. CountryCode: {result.Id}",
                    module: "Admin Security Settings"
                );

                await _mediator.Publish(domainEvent, cancellationToken);
                _logger.LogInformation($"Published AuditLogsDomainEvent for AdminSecuritySettings entity with ID: { result.Id}.");

                var AdminSettingsDto = _mapper.Map<AdminSecuritySettingsDto>(result);
                _logger.LogDebug("Mapped AdminSecuritySettings entity to AdminSecuritySettingsDto.");

            
                 return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "AdminSecuritySettings created successfully",
                Data = AdminSettingsDto.Id
            };
    
       
       
        }




    }


}