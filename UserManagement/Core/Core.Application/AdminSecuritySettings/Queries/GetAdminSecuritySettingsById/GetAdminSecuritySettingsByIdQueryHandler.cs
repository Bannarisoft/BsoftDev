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

namespace Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettingsById
{
    public class GetAdminSecuritySettingsByIdQueryHandler :IRequestHandler<GetAdminSecuritySettingsByIdQuery, ApiResponseDTO<AdminSecuritySettingsDto>>
    {

    
          private readonly IAdminSecuritySettingsQueryRepository _IAdminSecuritySettingsQueryRepository;        
        private readonly IMapper _mapper;
         private readonly IMediator _mediator;
          private readonly ILogger<GetAdminSecuritySettingsByIdQueryHandler> _logger;

    

    public GetAdminSecuritySettingsByIdQueryHandler(IAdminSecuritySettingsQueryRepository  adminSecuritySettingsQueryRepository,IMapper mapper , IMediator mediator,ILogger<GetAdminSecuritySettingsByIdQueryHandler> logger)
         {
            _IAdminSecuritySettingsQueryRepository = adminSecuritySettingsQueryRepository;
            _mapper =mapper;
            _mediator = mediator;
            _logger = logger;
        } 

           public async Task<ApiResponseDTO<AdminSecuritySettingsDto>> Handle(GetAdminSecuritySettingsByIdQuery request, CancellationToken cancellationToken)
        {
             _logger.LogInformation("Handling GetAdminSecuritySettingsByIdQuery for ID: {Id}", request.Id);
            // Fetch admin security setting by ID
                var adminSettings = await _IAdminSecuritySettingsQueryRepository.GetAdminSecuritySettingsByIdAsync(request.Id);
                 if (adminSettings == null)
                    {
                        _logger.LogWarning("Department with ID {DepartmentId} not found.", request.Id);

                        return new ApiResponseDTO<AdminSecuritySettingsDto>
                        {
                            IsSuccess = false,
                            Message = "Department not found.",
                            Data = null
                        };
                    }

        
                _logger.LogInformation("Admin Security Settings with ID {Id} retrieved successfully. Mapping to DTO.", request.Id);

                // Map to DTO
                var adminSettingsDto = _mapper.Map<AdminSecuritySettingsDto>(adminSettings);

                // Publish domain event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: adminSettingsDto.Id.ToString(),
                    actionName: adminSettingsDto.Id.ToString(),
                    details: $"Admin Security Setting with ID '{adminSettingsDto.Id}' was fetched.",
                    module: "Admin Security Settings"
                );

                _logger.LogInformation("AuditLogsDomainEvent published for Admin Security Settings with ID {Id}.", request.Id);

             
                   await _mediator.Publish(domainEvent, cancellationToken);
                  return new ApiResponseDTO<AdminSecuritySettingsDto> { IsSuccess = true, Message = "Success", Data = adminSettingsDto };
        

          
        }


    }
}