using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineGroupUser;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineGroupUser.Queries.GetMachineGroupUser
{
    public class GetMachineGroupUserQueryHandler : IRequestHandler<GetMachineGroupUserQuery, ApiResponseDTO<List<MachineGroupUserDto>>>
    {
        private readonly IMachineGroupUserQueryRepository _machineGroupQuery;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetMachineGroupUserQueryHandler(IMachineGroupUserQueryRepository machineGroupQuery, IMapper mapper, IMediator mediator)
        {
            _machineGroupQuery = machineGroupQuery;
            _mapper = mapper;
            _mediator = mediator;
            
        }
        public async Task<ApiResponseDTO<List<MachineGroupUserDto>>> Handle(GetMachineGroupUserQuery request, CancellationToken cancellationToken)
        {
              var (machineGroupUser, totalCount) = await _machineGroupQuery.GetAllMachineGroupUserAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var machineGroupUserList = _mapper.Map<List<MachineGroupUserDto>>(machineGroupUser);

             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetMachineGroupUser",
                    actionCode: "",        
                    actionName: "",
                    details: $"MachineGroupUser details was fetched.",
                    module:"MachineGroupUser"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<MachineGroupUserDto>> 
            { 
                IsSuccess = true, 
                Message = "Success", 
                Data = machineGroupUserList ,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
                };
        }
    }
}