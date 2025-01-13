using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces.IState;
using Core.Application.State.Queries.GetStates;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.State.Queries.GetCountries
{
    public class GetStateQueryHandler : IRequestHandler<GetStateQuery, Result<List<StateDto>>>
    {
        private readonly IStateQueryRepository _stateRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetStateQueryHandler(IStateQueryRepository stateRepository , IMapper mapper, IMediator mediator)
        {
            _stateRepository = stateRepository;  
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<Result<List<StateDto>>> Handle(GetStateQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var states = await _stateRepository.GetAllStatesAsync();
                var statesList = _mapper.Map<List<StateDto>>(states);            
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "",
                    details: $"State details was fetched.",
                    module:"State"
                );

                await _mediator.Publish(domainEvent, cancellationToken);
        
                return Result<List<StateDto>>.Success(statesList);
            }
            catch (Exception ex)
            {
                return Result<List<StateDto>>.Failure($"An error occurred while fetching the states: {ex.Message}");
            }
        }
    }
}