using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineMaster.Queries.GetMachineMaster
{
    public class GetMachineMasterQueryHandler : IRequestHandler<GetMachineMasterQuery, ApiResponseDTO<List<MachineMasterDto>>>
    {
        private readonly IMachineMasterQueryRepository _imachineMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;


        public GetMachineMasterQueryHandler(IMachineMasterQueryRepository imachineMasterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _imachineMasterQueryRepository = imachineMasterQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<MachineMasterDto>>> Handle(GetMachineMasterQuery request, CancellationToken cancellationToken)
        {
            var MachineMaster = await _imachineMasterQueryRepository.GetAllMachineAsync(request.SearchTerm);
            var machineMastersgroup = _mapper.Map<List<MachineMasterDto>>(MachineMaster);

            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetMachineMasterQuery",
                actionCode: "Get",
                actionName: MachineMaster.Count.ToString(),
                details: "MachineMaster details were fetched.",
                module: "MachineMaster"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<List<MachineMasterDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = machineMastersgroup,
                TotalCount = MachineMaster.Count
            };
        }
    }
}