using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces.IState;
using Core.Application.State.Queries.GetStates;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.State.Commands.DeleteState
{
  public class DeleteStateCommandHandler : IRequestHandler<DeleteStateCommand, Result<StateDto>>
    {
        private readonly IStateCommandRepository _stateRepository;
        private readonly IStateQueryRepository _stateQueryRepository;
        private readonly IMapper _mapper;

        public DeleteStateCommandHandler(IStateCommandRepository stateRepository, IMapper mapper,IStateQueryRepository stateQueryRepository)
        {
            _stateRepository = stateRepository;
            _mapper = mapper;
            _stateQueryRepository = stateQueryRepository;
        }

        public async Task<Result<StateDto>> Handle(DeleteStateCommand request, CancellationToken cancellationToken)
        {
            var state = await _stateQueryRepository.GetByIdAsync(request.Id);
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