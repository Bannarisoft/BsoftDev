using MediatR;
using Core.Application.State.Queries.GetStates;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces.IState;
using Core.Domain.Events;

namespace Core.Application.State.Queries.GetStateAutoComplete
{
    public class GetStateAutoCompleteQueryHandler : IRequestHandler<GetStateAutoCompleteQuery, Result<List<StateDto>>>
    
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

        public async Task<Result<List<StateDto>>> Handle(GetStateAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _stateRepository.GetByStateNameAsync(request.SearchPattern);
                if (!result.IsSuccess || result.Data == null || !result.Data.Any())
                {
                    return Result<List<StateDto>>.Failure("No States found matching the search pattern.");
                }
                var stateDto = _mapper.Map<List<StateDto>>(result.Data);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAutoComplete",
                    actionCode:"",        
                    actionName: request.SearchPattern,                
                    details: $"State '{request.SearchPattern}' was searched",
                    module:"State"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
                return Result<List<StateDto>>.Success(stateDto);         
            }
            catch (Exception ex)
            {
                return Result<List<StateDto>>.Failure($"An error occurred while fetching the states: {ex.Message}");
            }
        }
    }
    
}