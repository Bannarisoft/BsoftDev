using MediatR;
using Core.Domain.Entities;
using AutoMapper;
using Core.Application.State.Queries.GetStates;
using Core.Application.Common;
using Core.Application.Common.Interfaces.IState;
using Core.Domain.Events;

namespace Core.Application.State.Commands.UpdateState
{    
    public class UpdateStateCommandHandler : IRequestHandler<UpdateStateCommand, Result<StateDto>>
    {
        private readonly IStateCommandRepository _stateRepository;
        private readonly IMapper _mapper;
        private readonly IStateQueryRepository _stateQueryRepository;
        private readonly IMediator _mediator; 

        public UpdateStateCommandHandler(IStateCommandRepository stateRepository, IMapper mapper, IStateQueryRepository stateQueryRepository, IMediator mediator)
        {
            _stateRepository = stateRepository;
             _mapper = mapper;
            _stateQueryRepository = stateQueryRepository;
            _mediator = mediator;
        }
        
        public async Task<Result<StateDto>> Handle(UpdateStateCommand request, CancellationToken cancellationToken)
        {
            var state = await _stateQueryRepository.GetByIdAsync(request.Id);
             if (state == null)
                return Result<StateDto>.Failure("State not found.");

            var oldStateName = state.StateName;
            state.StateName = request.StateName;
            if (state == null || state.IsActive != 1)
            {
                return Result<StateDto>.Failure("Invalid StateID. The specified State does not exist or is inactive.");
            }

            var countryExists = await _stateRepository.CountryExistsAsync(request.CountryId);
            if (!countryExists)
            {
                return Result<StateDto>.Failure("Invalid CountryID. The specified Country does not exist or is inactive.");
            }

            var stateExists = await _stateRepository.GetStateByCodeAsync(request.StateCode, request.CountryId);
            if (stateExists)
            {
                return Result<StateDto>.Failure("StateCode already exists in the specified Country.");
            }

            // Map the request to the Cities entity
            var updatedStateEntity = _mapper.Map<States>(request);

            try
            {
                var updateResult = await _stateRepository.UpdateAsync(request.Id, updatedStateEntity);

                // Fetch the updated city to map to the DTO
                var updatedState = await _stateQueryRepository.GetByIdAsync(request.Id);                                                
                    if (updatedState != null)
                    {                    
                        var stateDto = _mapper.Map<StateDto>(updatedState);
                        //Domain Event
                        var domainEvent = new AuditLogsDomainEvent(
                            actionDetail: "Update",
                            actionCode: stateDto.StateCode,
                            actionName: stateDto.StateName,                            
                            details: $"State '{oldStateName}' was updated to '{stateDto.StateName}'.  StateCode: {stateDto.StateCode}",
                            module:"State"
                        );
                
                        await _mediator.Publish(domainEvent, cancellationToken);

                        return Result<StateDto>.Success(stateDto);

                    }
                    else
                    {
                        return Result<StateDto>.Failure("State update failed.");
                    }              
            }
            catch (Exception ex)
            {
                return Result<StateDto>.Failure($"An error occurred while updating the state: {ex.Message}");
            }        
        }    
    }
}