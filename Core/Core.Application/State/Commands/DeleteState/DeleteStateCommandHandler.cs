using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interface;
using Core.Application.State.Queries.GetStates;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.State.Commands.DeleteState
{
  public class DeleteStateCommandHandler : IRequestHandler<DeleteStateCommand, Result<StateDto>>
    {
        private readonly IStateRepository _stateRepository;
        private readonly IMapper _mapper;

        public DeleteStateCommandHandler(IStateRepository stateRepository, IMapper mapper)
        {
            _stateRepository = stateRepository;
            _mapper = mapper;
        }

        public async Task<Result<StateDto>> Handle(DeleteStateCommand request, CancellationToken cancellationToken)
        {
            var state = await _stateRepository.GetByIdAsync(request.Id);
            if (state == null || state.IsActive != 1)
            {
                return Result<StateDto>.Failure("Invalid StateID. The specified State does not exist or is inactive.");
            }

            // Mark the city as inactive (soft delete)
            var stateUpdate = new States
            {
                Id = request.Id,
                StateCode = state.StateCode, // Preserve original CityCode
                StateName = state.StateName, // Preserve original CityName
                CountryId = state.CountryId,
                IsActive = 0
            };

            var updateResult = await _stateRepository.DeleteAsync(request.Id, stateUpdate);
            if (updateResult > 0)
            {
               var stateDto = _mapper.Map<StateDto>(stateUpdate);               
                return Result<StateDto>.Success(stateDto);
            }

            return Result<StateDto>.Failure("State deletion failed.");
        }
    }
}