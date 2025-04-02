using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineGroupUser;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineGroupUser.Queries.GetMachineGroupUserAutoComplete
{
    public class GetMachineGroupUserAutoCompleteQueryHandler : IRequestHandler<GetMachineGroupUserAutoCompleteQuery, ApiResponseDTO<List<MachineGroupUserAutoCompleteDto>>>
    {
        private readonly IMachineGroupUserQueryRepository _machineGroupQuery;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetMachineGroupUserAutoCompleteQueryHandler(IMachineGroupUserQueryRepository machineGroupQuery, IMapper mapper, IMediator mediator)
        {
            _machineGroupQuery = machineGroupQuery;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<List<MachineGroupUserAutoCompleteDto>>> Handle(GetMachineGroupUserAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var result = await _machineGroupQuery.GetMachineGroupUserByName(request.SearchPattern ?? string.Empty);
            var machineGroupResult = _mapper.Map<List<MachineGroupUserAutoCompleteDto>>(result);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "",
                    details: $"MachineGroup User details was fetched.",
                    module:"MachineGroup User"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<MachineGroupUserAutoCompleteDto>> 
            { 
                IsSuccess = true, 
                Message = "Success", 
                Data = machineGroupResult 
             };   
        }
    }
}