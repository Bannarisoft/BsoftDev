using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.ActivityCheckListMaster.Queries.GetActivityCheckListMaster;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IActivityCheckListMaster;
using Core.Domain.Events;
using MassTransit;
using MediatR;

namespace Core.Application.ActivityCheckListMaster.Command.CreateActivityCheckListMaster
{
    public class CreateActivityCheckListMasterCommandHandler : IRequestHandler<CreateActivityCheckListMasterCommand, ApiResponseDTO<int>>
    {

         private readonly IActivityCheckListMasterCommandRepository _activityCheckListMasterCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;

        public CreateActivityCheckListMasterCommandHandler(IActivityCheckListMasterCommandRepository activityCheckListMasterCommandRepository, IMediator imediator, IMapper imapper)
        {
            _activityCheckListMasterCommandRepository = activityCheckListMasterCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
        }

        public async Task<ApiResponseDTO<int>> Handle(CreateActivityCheckListMasterCommand request, CancellationToken cancellationToken)
        {
             var activityCheckListMaster = _imapper.Map<Core.Domain.Entities.ActivityCheckListMaster>(request);
            
            var result = await _activityCheckListMasterCommandRepository.CreateAsync(activityCheckListMaster);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: activityCheckListMaster.ActivityId.ToString(),
                actionName: "ActivityChecklist",
                details: $"MachineMaster details was created",
                module: "MachineMaster");
            await _imediator.Publish(domainEvent, cancellationToken);
          
            var activityCheckListMasterDto = _imapper.Map<Core.Domain.Entities.ActivityCheckListMaster>(activityCheckListMaster);
            if (result != null)
                  {
                    
                        return new ApiResponseDTO<int>
                       {
                           IsSuccess = true,
                           Message = "ActivityChecklist created successfully",
                           Data = result.ActivityId
                      };
                 }
            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "ActivityChecklist Creation Failed",
                Data = result.ActivityId
            };
        }
    }
}