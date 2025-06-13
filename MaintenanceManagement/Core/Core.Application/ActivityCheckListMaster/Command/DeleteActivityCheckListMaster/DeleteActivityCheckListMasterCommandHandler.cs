using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IActivityCheckListMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.ActivityCheckListMaster.Command.DeleteActivityCheckListMaster
{
    public class DeleteActivityCheckListMasterCommandHandler : IRequestHandler<DeleteActivityCheckListMasterCommand, ApiResponseDTO<int>>
    {
        private readonly IActivityCheckListMasterCommandRepository _activityCheckListMasterCommandRepository;
         private readonly IMediator _mediator; 
        private readonly IMapper _imapper;


        public DeleteActivityCheckListMasterCommandHandler(IActivityCheckListMasterCommandRepository activityCheckListMasterCommandRepository, IMediator imediator, IMapper imapper)
        {
            _activityCheckListMasterCommandRepository = activityCheckListMasterCommandRepository;
            _mediator = imediator;
            _imapper = imapper;
        }
        
           public async Task<ApiResponseDTO<int>> Handle(DeleteActivityCheckListMasterCommand request, CancellationToken cancellationToken)
        {
          

            var activityCheckListMaster = _imapper.Map<Core.Domain.Entities.ActivityCheckListMaster>(request);
            var result = await _activityCheckListMasterCommandRepository.DeleteAsync(request.Id,activityCheckListMaster);
            if (result == -1) 
            {
         
             return new ApiResponseDTO<int> { IsSuccess = false, Message = "ActivityChecklist not found."};
            }

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: activityCheckListMaster.ActivityCheckList,
                actionName:  "ActivityChecklist",   
                details: $"ActivityChecklist details was deleted",
                module: "ActivityChecklist");
            await _mediator.Publish(domainEvent);
          

            return new ApiResponseDTO<int>
            {
                IsSuccess = true,   
                Data = result,
                Message = "ActivityChecklist deleted successfully."
    
            };
        }
        
    }
}