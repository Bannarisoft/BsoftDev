
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineGroupUser;
using Core.Application.MachineGroupUser.Queries.GetMachineGroupUser;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineGroupUser.Queries.GetMachineGroupUserById
{
    public class GetMachineGroupUserByIdQueryHandler : IRequestHandler<GetMachineGroupUserByIdQuery, ApiResponseDTO<MachineGroupUserDto>>
    {
        private readonly IMachineGroupUserQueryRepository _machineGroupQuery;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetMachineGroupUserByIdQueryHandler(IMachineGroupUserQueryRepository machineGroupQuery, IMapper mapper, IMediator mediator)
        {
            _machineGroupQuery = machineGroupQuery;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<MachineGroupUserDto>> Handle(GetMachineGroupUserByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _machineGroupQuery.GetByIdAsync(request.Id);
            var machineGroupResult = _mapper.Map<MachineGroupUserDto>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "",        
                    actionName: "",
                    details: $"MachineGroup User details was fetched.",
                    module:"MachineGroup User "
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<MachineGroupUserDto> 
          { 
            IsSuccess = true, 
            Message = "Success", 
            Data = machineGroupResult 
            };
        }
    }
}