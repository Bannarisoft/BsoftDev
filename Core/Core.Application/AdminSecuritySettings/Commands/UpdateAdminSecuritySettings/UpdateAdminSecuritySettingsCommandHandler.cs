using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces.IAdminSecuritySettings;
using Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettings;
using Core.Domain.Events;
using MediatR;
using Core.Domain.Entities;

namespace Core.Application.AdminSecuritySettings.Commands.UpdateAdminSecuritySettings
{
    public class UpdateAdminSecuritySettingsCommandHandler      : IRequestHandler<UpdateAdminSecuritySettingsCommand ,Result<int>>
    {
       public readonly IAdminSecuritySettingsCommandRepository _IAdminSecuritySettingsCommandRepository;
       private readonly IMapper _Imapper; 
        
        private readonly IAdminSecuritySettingsQueryRepository _IAdminSecuritySettingsQueryRepository;
        private readonly IMediator _mediator; 

          public UpdateAdminSecuritySettingsCommandHandler(IAdminSecuritySettingsCommandRepository adminSecuritySettingsCommandRepository,IAdminSecuritySettingsQueryRepository adminSecuritySettingsQueryRepository , IMapper Imapper, IMediator mediator  )
        {
            _IAdminSecuritySettingsCommandRepository = adminSecuritySettingsCommandRepository;
            _IAdminSecuritySettingsQueryRepository = adminSecuritySettingsQueryRepository;
            _Imapper = Imapper;
           
             _mediator = mediator;
        }


         public async Task<Result<int>> Handle(UpdateAdminSecuritySettingsCommand request, CancellationToken cancellationToken)
       {

             // Fetch the existing department by ID
                var adminSecSettings  = await _IAdminSecuritySettingsQueryRepository.GetAdminSecuritySettingsByIdAsync(request.Id);
      


                if (adminSecSettings  == null || adminSecSettings .IsActive != 1)
                {
                    return Result<int>.Failure("Invalid Admin Settings Id. The specified Department does not exist or is inactive.");
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

                if (result == 0) // Assuming '0' indicates a failure
                {
                    return Result<int>.Failure("Failed to update the department.");
                }

         
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: request.Id.ToString(),
                     actionName: "Update Admin Security Settings",
                    details: $"Admin Settings  was updated. Admin Settings ID: {request.Id}",
                    module: "Admin Secutrity Settings"
                );
                await _mediator.Publish(domainEvent, cancellationToken);

           
                return Result<int>.Success(result);

        
         }


        
    }
}