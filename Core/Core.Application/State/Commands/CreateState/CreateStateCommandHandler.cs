using Core.Domain.Entities;
using AutoMapper;
using MediatR;
using Core.Application.State.Queries.GetStates;
using Core.Application.Common;
using Core.Application.Common.Interfaces.IState;
using Core.Domain.Events;

namespace Core.Application.State.Commands.CreateState
{    
     public class CreateStateCommandHandler : IRequestHandler<CreateStateCommand, Result<StateDto>>
     
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

        public async Task<Result<StateDto>> Handle(CreateStateCommand request, CancellationToken cancellationToken)
        {        
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
            // Map the CreateCityCommand to the City entity
            var stateEntity = _mapper.Map<States>(request);
            try
            {
                var result = await _stateRepository.CreateAsync(stateEntity);
                var stateDto = _mapper.Map<StateDto>(result);
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
                
                return Result<StateDto>.Success(stateDto);                
            }
            catch (Exception ex)
            {
                return Result<StateDto>.Failure($"An error occurred while creating the state: {ex.Message}");
            }
        }      
    }
}