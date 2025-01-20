using MediatR;
using Core.Application.State.Queries.GetStates;
using AutoMapper;
using Core.Application.Common.Interfaces.IState;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;

namespace Core.Application.State.Queries.GetStateAutoComplete
{
    public class GetStateAutoCompleteQueryHandler : IRequestHandler<GetStateAutoCompleteQuery, ApiResponseDTO<List<StateDto>>>    
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
        public async Task<ApiResponseDTO<List<StateDto>>> Handle(GetStateAutoCompleteQuery request, CancellationToken cancellationToken)
        {          
            var result = await _stateRepository.GetByStateNameAsync(request.SearchPattern);
            if (result == null || result.Count == 0)
            {
                 return new ApiResponseDTO<List<StateDto>>
                {
                    IsSuccess = false,
                    Message = "No States found matching the search pattern."
                };
            }
            var stateDto = _mapper.Map<List<StateDto>>(result);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAutoComplete",
                actionCode:"",        
                actionName: request.SearchPattern,                
                details: $"State '{request.SearchPattern}' was searched",
                module:"State"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<StateDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = stateDto
            };
        }
    }
}
    