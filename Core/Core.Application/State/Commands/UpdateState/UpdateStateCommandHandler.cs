using MediatR;
using Core.Domain.Entities;
using AutoMapper;
using Core.Application.State.Queries.GetStates;
using Core.Application.Common.Interfaces.IState;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;

namespace Core.Application.State.Commands.UpdateState
{    
    public class UpdateStateCommandHandler : IRequestHandler<UpdateStateCommand, ApiResponseDTO<StateDto>>
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
        public async Task<ApiResponseDTO<StateDto>> Handle(UpdateStateCommand request, CancellationToken cancellationToken)
        {
            var state = await _stateQueryRepository.GetByIdAsync(request.Id);
             if (state == null)                
                return new ApiResponseDTO<StateDto>
                {
                    IsSuccess = false,
                    Message = "State not found"
                };

            var oldStateName = state.StateName;
            state.StateName = request.StateName;
            if (state == null || state.IsActive != 1)
            {                
                return new ApiResponseDTO<StateDto>
                {
                    IsSuccess = false,
                    Message = "Invalid StateID. The specified State does not exist or is inactive."
                };
            }
            var countryExists = await _stateRepository.CountryExistsAsync(request.CountryId);
            if (!countryExists)
            {                
                return new ApiResponseDTO<StateDto>
                {
                    IsSuccess = false,
                    Message = "Invalid CountryID. The specified Country does not exist or is inactive."
                };
            }
            var stateExists = await _stateRepository.GetStateByCodeAsync(request.StateCode, request.CountryId);
            if (stateExists)
            {                
                return new ApiResponseDTO<StateDto>
                {
                    IsSuccess = false,
                    Message = "StateCode already exists in the specified Country."
                };
            }
            var updatedStateEntity = _mapper.Map<States>(request);          
            var updateResult = await _stateRepository.UpdateAsync(request.Id, updatedStateEntity);            
            
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
                if(updateResult>0)
                {
                    return new ApiResponseDTO<StateDto>
                    {
                        IsSuccess = true,
                        Message = "State updated successfully.",
                        Data = stateDto
                    };
                }
                return new ApiResponseDTO<StateDto>
                {
                    IsSuccess = false,
                    Message = "City not updated."
                };
            }
            else
            {
                return new ApiResponseDTO<StateDto>{
                IsSuccess = false,
                Message = "State not found."
                };
            }                
        }    
    }
}