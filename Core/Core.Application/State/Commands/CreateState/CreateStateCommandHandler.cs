using Core.Domain.Entities;
using AutoMapper;
using MediatR;
using Core.Application.State.Queries.GetStates;
using Core.Application.Common.Interfaces.IState;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;

namespace Core.Application.State.Commands.CreateState
{    
    public class CreateStateCommandHandler : IRequestHandler<CreateStateCommand, ApiResponseDTO<StateDto>>     
    {
        private readonly IMapper _mapper;
        private readonly IStateCommandRepository _stateRepository;
        private readonly IMediator _mediator; 

        // Constructor Injection
        public CreateStateCommandHandler(IMapper mapper, IStateCommandRepository stateRepository, IMediator mediator)
        {
            _mapper = mapper;
            _stateRepository = stateRepository;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<StateDto>> Handle(CreateStateCommand request, CancellationToken cancellationToken)
        {        
            var countryExists = await _stateRepository.CountryExistsAsync(request.CountryId);
            if (!countryExists)
            {
                  return new ApiResponseDTO<StateDto>{
                    IsSuccess = false, 
                    Message = "Invalid CountryId. The specified country does not exist or is inactive."
                    };                     
            }
            var stateExists = await _stateRepository.GetStateByCodeAsync(request.StateCode, request.CountryId);
            if (stateExists)
            {
                return new ApiResponseDTO<StateDto>{
                IsSuccess = false, 
                Message = "StateCode already exists in the specified city."
                };
            }            
            var stateEntity = _mapper.Map<States>(request);
            var result = await _stateRepository.CreateAsync(stateEntity);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: result.StateCode,
                actionName: result.StateName,
                details: $"State '{result.StateName}' was created. StateCode: {result.StateCode}",
                module:"State"
            );

            //var domainEvent = new StateDomainEvent(result.Id, result.StateName);
            await _mediator.Publish(domainEvent, cancellationToken);
            var stateDto = _mapper.Map<StateDto>(result);
            if (stateDto.Id > 0)
            {
                return new ApiResponseDTO<StateDto>{
                    IsSuccess = true, 
                    Message = "State created successfully.",
                    Data = stateDto
                };
            }
            return  new ApiResponseDTO<StateDto>{
                IsSuccess = false, 
                Message = "State not created."
            };           
        }
      
    }
}