using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Application.MachineMaster.Queries.GetMachineMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineMaster.Queries.GetMachineMasterAutoComplete
{
    public class GetMachineMasterAutoCompleteQueryHandler : IRequestHandler<GetMachineMasterAutoCompleteQuery,ApiResponseDTO<List<MachineMasterAutoCompleteDto>>>
    {
        private readonly IMachineMasterQueryRepository _imachineMasterQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetMachineMasterAutoCompleteQueryHandler(IMachineMasterQueryRepository imachineMasterQueryRepository,IMapper mapper,IMediator mediator)
        {
            _imachineMasterQueryRepository = imachineMasterQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<MachineMasterAutoCompleteDto>>> Handle(GetMachineMasterAutoCompleteQuery request, CancellationToken cancellationToken)
        {
             var result = await _imachineMasterQueryRepository.GetMachineAsync(request.SearchPattern);
            var machineMasters = _mapper.Map<List<MachineMasterAutoCompleteDto>>(result);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "GetMachineMasterAutoCompleteQuery",        
                    actionName: machineMasters.Count.ToString(),
                    details: $"MachineMaster details was fetched.",
                    module:"MachineMaster"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<MachineMasterAutoCompleteDto>> { IsSuccess = true, Message = "Success", Data = machineMasters };
        }
    }
}