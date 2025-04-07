using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IActivityCheckListMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.ActivityCheckListMaster.Command.UpdateActivityCheckListMaster
{
    public class UpdateActivityCheckListMasterCommandHandler :  IRequestHandler<UpdateActivityCheckListMasterCommand, ApiResponseDTO<int>>
    {
         private readonly IActivityCheckListMasterCommandRepository _activityChecklistRepo;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

         public UpdateActivityCheckListMasterCommandHandler( IActivityCheckListMasterCommandRepository activityChecklistRepo, IMediator mediator,IMapper mapper)
        {
            _activityChecklistRepo = activityChecklistRepo;
            _mediator = mediator;
            _mapper = mapper;
        }
         public async Task<ApiResponseDTO<int>> Handle(UpdateActivityCheckListMasterCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Core.Domain.Entities.ActivityCheckListMaster>(request);

            var result = await _activityChecklistRepo.UpdateAsync(request.Id, entity);

           if (!result)
            {
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Activity Checklist not found."
                };
            }

            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode:  request.Id.ToString() ,
                actionName: entity.ActivityCheckList ?? "NULL",
                details: $"Activity Checklist was updated",
                module: "ActivityCheckListMaster");

            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Activity Checklist Updated Successfully.",
                    Data = request.Id
                };
          }


    }
}