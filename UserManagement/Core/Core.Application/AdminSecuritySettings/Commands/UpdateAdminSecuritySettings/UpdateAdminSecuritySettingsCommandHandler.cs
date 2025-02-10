using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces.IAdminSecuritySettings;
using Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettings;
using Core.Domain.Events;
using MediatR;
using Core.Domain.Entities;
using Core.Application.Common.HttpResponse;
using Microsoft.Extensions.Logging;

namespace Core.Application.AdminSecuritySettings.Commands.UpdateAdminSecuritySettings
{
    public class UpdateAdminSecuritySettingsCommandHandler      : IRequestHandler<UpdateAdminSecuritySettingsCommand ,ApiResponseDTO<int>>
    {
       public readonly IAdminSecuritySettingsCommandRepository _IAdminSecuritySettingsCommandRepository;
       private readonly IMapper _Imapper; 
        
        private readonly IAdminSecuritySettingsQueryRepository _IAdminSecuritySettingsQueryRepository;
        private readonly IMediator _mediator; 
         private readonly ILogger<UpdateAdminSecuritySettingsCommandHandler> _logger;

          public UpdateAdminSecuritySettingsCommandHandler(IAdminSecuritySettingsCommandRepository adminSecuritySettingsCommandRepository,IAdminSecuritySettingsQueryRepository adminSecuritySettingsQueryRepository , IMapper Imapper, IMediator mediator ,ILogger<UpdateAdminSecuritySettingsCommandHandler> logger)
        {
            _IAdminSecuritySettingsCommandRepository = adminSecuritySettingsCommandRepository;
            _IAdminSecuritySettingsQueryRepository = adminSecuritySettingsQueryRepository;
            _Imapper = Imapper;
            _logger = logger;           
             _mediator = mediator;
        }

        public async Task<ApiResponseDTO<int>> Handle(UpdateAdminSecuritySettingsCommand request, CancellationToken cancellationToken)
        {
             // Fetch the existing adminSecuritySettings by ID
                var adminSecSettings  = await _IAdminSecuritySettingsQueryRepository.GetAdminSecuritySettingsByIdAsync(request.Id);
      
                if (adminSecSettings  is null )
                {
                 
                   _logger.LogWarning($" Admin Settings with ID {request.Id} not found.");
                return new ApiResponseDTO<int >
                {
                    IsSuccess = false,
                    Message = "AdminSecuritySettings not found/AdminSecuritySettings is deleted "
                };
                }                
              var adminsettingsentity = _Imapper.Map<Core.Domain.Entities.AdminSecuritySettings>(request);

              var result = await _IAdminSecuritySettingsCommandRepository.UpdateAsync(request.Id, adminsettingsentity);
            
             if (result == -1) // Entity not found
            {
                _logger.LogWarning($"Failed to update AdminSecuritySettings with ID {request.Id }.");
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Failed to update AdminSecuritySettings"
                };
            }

            _logger.LogInformation($"AdminSecuritySettings with ID { request.Id} updated successfully.");

            // Map the updated entity to DTO
            var adminsetting = await _IAdminSecuritySettingsQueryRepository.GetAdminSecuritySettingsByIdAsync(request.Id);
            var adminsettingsDto = _Imapper.Map<AdminSecuritySettingsDto>(adminsetting);

         
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: $"Update Admin Security Settings{adminsettingsDto.Id}" ,
                     actionName: "Update Admin Security Settings",
                    details: $"Admin Settings  was updated. Admin Settings ID: {request.Id}",
                    module: "Admin Secutrity Settings"
                );
               
                 await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation($"AuditLogsDomainEvent published for AdminSecuritySettings ID {adminSecSettings.Id}.");

            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "AdminSecuritySettings updated successfully",
                Data=result
               
            };

        
        }


        
    }
}