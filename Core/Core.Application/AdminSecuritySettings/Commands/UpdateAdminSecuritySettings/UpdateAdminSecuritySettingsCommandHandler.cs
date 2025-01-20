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
    public class UpdateAdminSecuritySettingsCommandHandler      : IRequestHandler<UpdateAdminSecuritySettingsCommand ,ApiResponseDTO<AdminSecuritySettingsDto>>
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


         public async Task<ApiResponseDTO<AdminSecuritySettingsDto>> Handle(UpdateAdminSecuritySettingsCommand request, CancellationToken cancellationToken)
       {

             // Fetch the existing department by ID
                var adminSecSettings  = await _IAdminSecuritySettingsQueryRepository.GetAdminSecuritySettingsByIdAsync(request.Id);
      


                if (adminSecSettings  == null )
                {
                  //  return ApiResponseDTO<AdminSecuritySettingsDto>.Failure("Invalid Admin Settings Id. The specified Department does not exist or is inactive.");
                   _logger.LogWarning("Department with ID {DepartmentId} not found.", request.Id);
                return new ApiResponseDTO<AdminSecuritySettingsDto>
                {
                    IsSuccess = false,
                    Message = "AdminSecuritySettings not found"
                };
                }

                // Map the request to the department entity
                  adminSecSettings.Id = request.Id; // Update properties based on the request
                 adminSecSettings.PasswordHistoryCount = request.PasswordHistoryCount;
                 adminSecSettings.SessionTimeoutMinutes = request.SessionTimeoutMinutes;
                  adminSecSettings.MaxFailedLoginAttempts=request.MaxFailedLoginAttempts;
                  adminSecSettings.AccountAutoUnlockMinutes=request.AccountAutoUnlockMinutes;
                  adminSecSettings.PasswordExpiryDays=request.PasswordExpiryDays;
                  adminSecSettings.PasswordExpiryAlertDays=request.PasswordExpiryAlertDays;
                  adminSecSettings.IsTwoFactorAuthenticationEnabled=request.IsTwoFactorAuthenticationEnabled;
                  adminSecSettings.MaxConcurrentLogins=request.MaxConcurrentLogins;
                  adminSecSettings.IsForcePasswordChangeOnFirstLogin=request.IsForcePasswordChangeOnFirstLogin;
                  adminSecSettings.PasswordResetCodeExpiryMinutes=request.PasswordResetCodeExpiryMinutes;
                  adminSecSettings.PasswordResetCodeExpiryMinutes=request.PasswordResetCodeExpiryMinutes;
                  adminSecSettings.IsCaptchaEnabledOnLogin=request.IsCaptchaEnabledOnLogin;
             
                 var result = await _IAdminSecuritySettingsCommandRepository.UpdateAsync(request.Id, adminSecSettings);

                if (result == null)
            {
                _logger.LogWarning("Failed to update Department with ID {DepartmentId}.", request.Id);
                return new ApiResponseDTO<AdminSecuritySettingsDto>
                {
                    IsSuccess = false,
                    Message = "Failed to update department"
                };
            }

            _logger.LogInformation("Department with ID {DepartmentId} updated successfully.", request.Id);

            // Map the updated entity to DTO
            var dept = await _IAdminSecuritySettingsQueryRepository.GetAdminSecuritySettingsByIdAsync(request.Id);
            var departmentDto = _Imapper.Map<AdminSecuritySettingsDto>(dept);

         
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: request.Id.ToString(),
                     actionName: "Update Admin Security Settings",
                    details: $"Admin Settings  was updated. Admin Settings ID: {request.Id}",
                    module: "Admin Secutrity Settings"
                );
                // await _mediator.Publish(domainEvent, cancellationToken);

           
                // return Result<int>.Success(result);
                 await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation("AuditLogsDomainEvent published for Department ID {DepartmentId}.", adminSecSettings.Id);

            return new ApiResponseDTO<AdminSecuritySettingsDto>
            {
                IsSuccess = true,
                Message = "Department updated successfully"
               
            };

        
         }


        
    }
}