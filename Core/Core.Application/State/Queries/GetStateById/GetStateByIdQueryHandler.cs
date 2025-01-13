using MediatR;
using Core.Application.State.Queries.GetStates;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces.IState;
using Core.Domain.Events;

namespace Core.Application.State.Queries.GetStateById
{
    public class GetStateByIdQueryHandler : IRequestHandler<GetStateByIdQuery, Result<StateDto>>
    {
        private readonly IStateQueryRepository _stateRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
    public GetStateByIdQueryHandler(IStateQueryRepository stateRepository, IMapper mapper, IMediator mediator)
        {
            _stateRepository = stateRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<Result<StateDto>> Handle(GetStateByIdQuery request, CancellationToken cancellationToken)
        {          
            try
            {
                var state = await _stateRepository.GetByIdAsync(request.Id);
                if (state == null)
                {
                    return Result<StateDto>.Failure($"Country with ID {request.Id} not found.");
                }
                
                var stateDto = _mapper.Map<StateDto>(state);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: stateDto.StateCode,        
                    actionName: stateDto.StateName,                
                    details: $"State '{stateDto.StateName}' was created. StateCode: {stateDto.StateCode}",
                    module:"State"
                );

                await _mediator.Publish(domainEvent, cancellationToken);
                return Result<StateDto>.Success(stateDto);
            }
            catch (Exception ex)
            {
                // Handle any unexpected exceptions
                return Result<StateDto>.Failure($"An error occurred while fetching the state: {ex.Message}");
            }
        }
    }
}