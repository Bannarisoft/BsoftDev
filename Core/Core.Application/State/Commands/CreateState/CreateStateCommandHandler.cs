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
            //var stateExists = await _stateRepository.GetStateByCodeAsync(request.StateName,request.StateCode, request.CountryId);
            var stateExists = await _stateRepository.GetStateByCodeAsync(request.StateName ?? string.Empty, 
                request.StateCode ?? string.Empty,      request.CountryId ) ;
            if (stateExists!= null)
            {
                return new ApiResponseDTO<StateDto>{
                IsSuccess = false, 
                Message = "StateName & Code already exists in the specified country."
                };
            }            
            var stateEntity = _mapper.Map<States>(request);
            var result = await _stateRepository.CreateAsync(stateEntity);
            //Domain Event
            if (result != null)
            {       
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Create",
                    actionCode: result.StateCode ?? string.Empty,
                    actionName: result.StateName ?? string.Empty,
                    details: $"State '{result.StateName}' was created. StateCode: {result.StateCode}",
                    module:"State"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            }
            
            
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