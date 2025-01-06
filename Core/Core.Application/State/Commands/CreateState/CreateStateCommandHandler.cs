using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Core.Application.State.Queries.GetStates;
using Core.Application.Common;
using Core.Application.Common.Interfaces.IState;

namespace Core.Application.State.Commands.CreateState
{    
     public class CreateStateCommandHandler : IRequestHandler<CreateStateCommand, Result<StateDto>>
     
    {
        private readonly IMapper _mapper;
        private readonly IStateCommandRepository _stateRepository;

        // Constructor Injection
        public CreateStateCommandHandler(IMapper mapper, IStateCommandRepository stateRepository)
        {
            _mapper = mapper;
            _stateRepository = stateRepository;
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
            var result = await _stateRepository.CreateAsync(stateEntity);

            // Map the result to CityDto and return success
            var stateDto = _mapper.Map<StateDto>(result);
            return Result<StateDto>.Success(stateDto);
        }      
    }
}