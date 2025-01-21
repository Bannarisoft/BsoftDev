using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AdminSecuritySettings.Commands.DeleteAdminSecuritySettings;
using Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettings;
using Core.Domain.Events;
using Core.Application.Common;
using AutoMapper;
using MediatR;
using Core.Application.Common.Interfaces.IAdminSecuritySettings;
using System.Threading;
using Core.Application.Common.HttpResponse;
using Microsoft.Extensions.Logging;


namespace Core.Application.AdminSecuritySettings.Commands.DeleteAdminSecuritySettings
{
    public class DeleteAdminSecuritySettingsCommandHandler  : IRequestHandler< DeleteAdminSecuritySettingsCommand  ,ApiResponseDTO<int>>
    {
          private readonly IAdminSecuritySettingsCommandRepository _IadminSecuritySettingsCommandRepository;  
       private readonly IMapper _Imapper;          
        private readonly IAdminSecuritySettingsQueryRepository  _IadminSecuritySettingsQueryRepository;
   
        private readonly IMediator _mediator; 
           private readonly ILogger<DeleteAdminSecuritySettingsCommandHandler> _logger;
       

        public DeleteAdminSecuritySettingsCommandHandler (IAdminSecuritySettingsCommandRepository adminSecuritySettingsCommandRepository ,IAdminSecuritySettingsQueryRepository adminSecuritySettingsQueryRepository ,IMediator mediator, IMapper mapper,ILogger<DeleteAdminSecuritySettingsCommandHandler> logger)
      {
         _IadminSecuritySettingsCommandRepository = adminSecuritySettingsCommandRepository;
         _Imapper = mapper;                       
          _IadminSecuritySettingsQueryRepository = adminSecuritySettingsQueryRepository;
          _mediator = mediator;
          _logger = logger;

      }

     public async Task<ApiResponseDTO<int>> Handle(DeleteAdminSecuritySettingsCommand request, CancellationToken cancellationToken)
      {       

          _logger.LogInformation("DeleteAdmin SecuritySettingsCommandHandler started for Admin Security Settings ID: {Id}", request.Id);

            // Check if department exists
            var department = await _IadminSecuritySettingsQueryRepository.GetAdminSecuritySettingsByIdAsync(request.Id);
            if (department == null)
            {
                _logger.LogWarning("Admin Security Settings with ID {Id} not found.", request.Id);
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Admin Security Settings not found",
                    Data = 0
                };
            }

              _logger.LogInformation("Admin Security Settings with ID {Id} found. Proceeding with deletion.", request.Id);

            // Map request to entity and delete
            var updatedDepartment = _Imapper.Map<Core.Domain.Entities.AdminSecuritySettings>(request.AdminSecuritySettingsStatusDto);
            var result = await _IadminSecuritySettingsCommandRepository.DeleteAsync(request.Id, updatedDepartment);

            if (result <= 0)
            {
                _logger.LogWarning("Failed to delete Admin Security Settings with ID {Id}.", request.Id);
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Failed to delete Admin Security Settings",
                    Data = result
                };
            }

            _logger.LogInformation("Admin Security Settings with ID {Id} deleted successfully.", request.Id);

            // Publish domain event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: updatedDepartment.Id.ToString(),
                actionName: "",
                details: $"Admin Security Settings ID: {request.Id} was changed to status inactive.",
                module: "Admin Security Settings"
            );

            await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation("AuditLogsDomainEvent published for Admin Security Settings ID {Id}.", request.Id);

            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "Admin Security Settings deleted successfully",
                Data = result
            };  
    //    _logger.LogInformation("Processing DeleteAdminSecuritySettingsCommand for ID: {Id}", request.Id);
    //       // Map the command to the Entity
    //             _logger.LogDebug("Mapping command to AdminSecuritySettings entity.");
    //             var adminsettings = _Imapper.Map<Core.Domain.Entities.AdminSecuritySettings>(request.AdminSecuritySettingsStatusDto);

    //             // Call repository to delete the entity
    //             _logger.LogInformation("Calling repository to delete Admin Security Settings with ID: {Id}", request.Id);
    //             var result = await _IadminSecuritySettingsCommandRepository.DeleteAsync(request.Id, adminsettings);

    //              if (result == null)
    //         {
    //             _logger.LogWarning("Failed to update Admin Security Settings with ID {Id}.", request.Id);
    //             return new ApiResponseDTO<int>
    //             {
    //                 IsSuccess = false,
    //                 Message = "Failed to update adminsecuritysettings"
    //             };
    //         }

    //         _logger.LogInformation("Admin Security Settings with ID {Id} updated successfully.", request.Id);

    //         // Map the updated entity to DTO
    //         var adminsecuritysetting = await _IadminSecuritySettingsQueryRepository.GetAdminSecuritySettingsByIdAsync(request.Id);
    //         var result = _Imapper.Map<AdminSecuritySettingsDto>(adminsecuritysetting);

    //             _logger.LogInformation("Successfully deleted Admin Security Settings with ID: {Id}.", request.Id);

    //             // Domain Event
    //             var domainEvent = new AuditLogsDomainEvent(
    //                 actionDetail: "Delete",
    //                 actionCode: request.Id.ToString(),
    //                 actionName: "",
    //                 details: $"Admin Security Settings Id: {request.Id} was changed to Status Inactive.",
    //                 module: "Admin Security Settings"
    //             );

               
    //              await _mediator.Publish(domainEvent, cancellationToken);
    //         _logger.LogInformation("AuditLogsDomainEvent published for Department ID {DepartmentId}.", adminsecuritysetting.Id);

    //         return new ApiResponseDTO<int>
    //         {
    //             IsSuccess = true,
    //             Message = "Department updated successfully"
               
    //         };
    //      // Map the command to the Entity
    //     var adminsettings = _Imapper.Map<Core.Domain.Entities.AdminSecuritySettings>(request.AdminSecuritySettingsStatusDto);
    //     // Call repository to delete the entity
    //     var result = await _IadminSecuritySettingsCommandRepository.DeleteAsync(request.Id, adminsettings);

    //    // Domain Event
    //     var domainEvent = new AuditLogsDomainEvent(
    //         actionDetail: "Delete",
    //         actionCode: request.Id.ToString(),
    //         actionName:"",
    //         details:$"Admin Security settings Id: {request.Id} was Changed to Status Inactive.",
    //         module:"Admin Security settings"
    //     );

    //     await _mediator.Publish(domainEvent, cancellationToken);
    //      return ApiResponseDTO<int>.Success(result); // Return the number of affected rows (e.g., 1 for success)
      }

    }
}