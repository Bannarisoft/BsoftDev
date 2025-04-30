using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IActivityMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.ActivityMaster.Queries.GetMachineGroupById
{
    public class GetMachineGroupNameByIdQueryHandler    : IRequestHandler<GetMachineGroupNameByIdQuery, ApiResponseDTO<List<GetMachineGroupNameByIdDto>>>
    {

         private readonly IActivityMasterQueryRepository _activityMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

         public GetMachineGroupNameByIdQueryHandler(IActivityMasterQueryRepository activityMasterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _activityMasterQueryRepository = activityMasterQueryRepository;
            _mapper =mapper;
            _mediator = mediator;
        }


         public async Task<ApiResponseDTO<List<GetMachineGroupNameByIdDto>>> Handle(GetMachineGroupNameByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _activityMasterQueryRepository.GetMachineGroupById(request.ActivityId);

        if (result == null || !result.Any()) // Check if the list is empty
        {
            return new ApiResponseDTO<List<GetMachineGroupNameByIdDto>>
            {
                IsSuccess = false,
                Message = $"Machine groups for ActivityMaster with Id {request.ActivityId} not found.",
                Data = null
            };
        }

        var machineGroups = _mapper.Map<List<GetMachineGroupNameByIdDto>>(result); // Map the list

        

        // Domain Event
        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "GetById",
            actionCode: "",
            actionName: "",
            details: $"Machine groups for ActivityMaster {request.ActivityId} were fetched.",
            module: "ActivityMaster"
        );

        await _mediator.Publish(domainEvent, cancellationToken);

        return new ApiResponseDTO<List<GetMachineGroupNameByIdDto>>
        {
            IsSuccess = true,
            Message = "Success",
            Data = machineGroups
        };
    }

        //  public async Task<ApiResponseDTO<GetMachineGroupNameByIdDto>> Handle(GetMachineGroupNameByIdQuery request, CancellationToken cancellationToken)
        // {
        //     var result = await _activityMasterQueryRepository.GetMachineGroupById(request.ActivityId);
            
        //     if (result is null)
        //     {
        //         return new ApiResponseDTO<GetMachineGroupNameByIdDto>
        //         {
        //             IsSuccess = false,
        //             Message = $"ActivityMaster with Id {request.ActivityId} not found.",
        //             Data = null
        //         };
        //     }
            
        //     var machineGroup = _mapper.Map<GetMachineGroupNameByIdDto>(result);

        //     // Domain Event
        //     var domainEvent = new AuditLogsDomainEvent(
        //         actionDetail: "GetById",
        //         actionCode: "",
        //         actionName: "",
        //         details: $"ActivityMaster details {machineGroup.ActivityId} were fetched.",
        //         module: "ActivityMaster"
        //     );

        //     await _mediator.Publish(domainEvent, cancellationToken);

        //     return new ApiResponseDTO<GetMachineGroupNameByIdDto>
        //     {
        //         IsSuccess = true,
        //         Message = "Success",
        //         Data = machineGroup
        //     };
        // }




        
    }
}