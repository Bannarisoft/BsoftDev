using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineGroup.Quries.GetMachineGroupAutoComplete
{
    public class GetMiscMasterAutoCompleteQueryHandler : IRequestHandler<GetMiscMasterAutoCompleteQuery,ApiResponseDTO<List<GetMachineGroupAutoCompleteDto>>>
    {

        private readonly IMachineGroupQueryRepository  _machineGroupQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;


         public GetMiscMasterAutoCompleteQueryHandler(IMachineGroupQueryRepository machineGroupQueryRepository , IMapper mapper, IMediator mediator)
         {
           
            _machineGroupQueryRepository = machineGroupQueryRepository;
            _mapper =mapper;
            _mediator = mediator;
         }

           public  async Task<ApiResponseDTO<List<GetMachineGroupAutoCompleteDto>>> Handle(GetMiscMasterAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var machineGroup  = await _machineGroupQueryRepository.GetMachineGroupAutoComplete(request.SearchPattern);

                    if (machineGroup == null || !machineGroup.Any())
            {
                return new ApiResponseDTO<List<GetMachineGroupAutoCompleteDto>>
                {
                    IsSuccess = false,
                    Message = $"No MachineGroup Masters found matching '{request.SearchPattern}'.",
                    Data = new List<GetMachineGroupAutoCompleteDto>()
                };
            }

            var division = _mapper.Map<List<GetMachineGroupAutoCompleteDto>>(machineGroup);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "", 
                    details: $"MachineGroup details was fetched.",
                    module:"MachineGroup"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<GetMachineGroupAutoCompleteDto>> { IsSuccess = true, Message = "Success", Data = division }; 
        }

        
    }
}