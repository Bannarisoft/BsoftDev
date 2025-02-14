using MediatR;
using Core.Application.State.Queries.GetStates;
using AutoMapper;
using Core.Application.Common.Interfaces.IState;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;

namespace Core.Application.State.Queries.GetStateAutoComplete
{
    public class GetStateAutoCompleteQueryHandler : IRequestHandler<GetStateAutoCompleteQuery, ApiResponseDTO<List<StateAutoCompleteDTO>>>    
    {
        private readonly IStateQueryRepository _stateRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        public GetStateAutoCompleteQueryHandler(IStateQueryRepository stateRepository,  IMapper mapper, IMediator mediator)
        {
            _stateRepository =stateRepository;
            _mapper =mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<List<StateAutoCompleteDTO>>> Handle(GetStateAutoCompleteQuery request, CancellationToken cancellationToken)
        {          
            var result = await _stateRepository.GetByStateNameAsync(request.SearchPattern ?? string.Empty);
            if (result is null || result.Count is 0)
            {
                 return new ApiResponseDTO<List<StateAutoCompleteDTO>>
                {
                    IsSuccess = false,
                    Message = "No States found matching the search pattern."
                };
            }
            var stateDto = _mapper.Map<List<StateAutoCompleteDTO>>(result);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAutoComplete",
                actionCode:"",        
                actionName: request.SearchPattern ?? string.Empty,                
                details: $"State '{request.SearchPattern}' was searched",
                module:"State"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<StateAutoCompleteDTO>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = stateDto
            };
        }
    }
}
    