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


namespace Core.Application.AdminSecuritySettings.Commands.DeleteAdminSecuritySettings
{
    public class DeleteAdminSecuritySettingsCommandHandler  : IRequestHandler< DeleteAdminSecuritySettingsCommand  ,Result<int>>
    {
          private readonly IAdminSecuritySettingsCommandRepository _IadminSecuritySettingsCommandRepository;  
       private readonly IMapper _Imapper;          
        private readonly IAdminSecuritySettingsQueryRepository  _IadminSecuritySettingsQueryRepository;
   
        private readonly IMediator _mediator; 
       

        public DeleteAdminSecuritySettingsCommandHandler (IAdminSecuritySettingsCommandRepository adminSecuritySettingsCommandRepository ,IAdminSecuritySettingsQueryRepository adminSecuritySettingsQueryRepository ,IMediator mediator, IMapper mapper)
      {
         _IadminSecuritySettingsCommandRepository = adminSecuritySettingsCommandRepository;
         _Imapper = mapper;                       
          _IadminSecuritySettingsQueryRepository = adminSecuritySettingsQueryRepository;
          _mediator = mediator;
      }

     public async Task<Result<int>> Handle(DeleteAdminSecuritySettingsCommand request, CancellationToken cancellationToken)
      {       
      
         // Map the command to the Entity
        var adminsettings = _Imapper.Map<Core.Domain.Entities.AdminSecuritySettings>(request.AdminSecuritySettingsStatusDto);
        // Call repository to delete the entity
        var result = await _IadminSecuritySettingsCommandRepository.DeleteAsync(request.Id, adminsettings);

       // Domain Event
        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "Delete",
            actionCode: request.Id.ToString(),
            actionName:"",
            details:$"Admin Security settings Id: {request.Id} was Changed to Status Inactive.",
            module:"Admin Security settings"
        );

        await _mediator.Publish(domainEvent, cancellationToken);
         return Result<int>.Success(result); // Return the number of affected rows (e.g., 1 for success)
      }

    }
}