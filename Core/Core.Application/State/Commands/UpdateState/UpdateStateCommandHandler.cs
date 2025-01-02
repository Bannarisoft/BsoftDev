using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interface;
using MediatR;
using Core.Domain.Entities;
using AutoMapper;
using Core.Application.State.Commands.UpdateState;
using Core.Application.State.Queries.GetStates;
using Core.Application.Common;

namespace Core.Application.State.Commands.UpdateState
{    
    public class UpdateStateCommandHandler : IRequestHandler<UpdateStateCommand, Result<StateDto>>
    {
        private readonly IStateRepository _stateRepository;
        private readonly IMapper _mapper;

        public UpdateStateCommandHandler(IStateRepository stateRepository, IMapper mapper)
        {
            _stateRepository = stateRepository;
             _mapper = mapper;
        }
        
        public async Task<Result<StateDto>> Handle(UpdateStateCommand request, CancellationToken cancellationToken)
        {
            var state = await _stateRepository.GetByIdAsync(request.Id);
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

            // Perform the update
            var updateResult = await _stateRepository.UpdateAsync(request.Id, updatedStateEntity);

            // Fetch the updated city to map to the DTO
            var updatedState = await _stateRepository.GetByIdAsync(request.Id);

            // If update was successful, map to DTO and return
            if (updatedState != null)
            {
                var stateDto = _mapper.Map<StateDto>(updatedState);
                return Result<StateDto>.Success(stateDto);
            }
            else
            {
                return Result<StateDto>.Failure("State update failed.");
            }
      
        }    
    }
}