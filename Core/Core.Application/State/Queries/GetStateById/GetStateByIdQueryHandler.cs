using MediatR;
using Core.Application.State.Queries.GetStates;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces.IState;

namespace Core.Application.State.Queries.GetStateById
{
    public class GetStateByIdQueryHandler : IRequestHandler<GetStateByIdQuery, Result<StateDto>>
    {
        private readonly IStateQueryRepository _stateRepository;
        private readonly IMapper _mapper;
    public GetStateByIdQueryHandler(IStateQueryRepository stateRepository, IMapper mapper)
        {
            _stateRepository = stateRepository;
            _mapper = mapper;
        }

        public async Task<Result<StateDto>> Handle(GetStateByIdQuery request, CancellationToken cancellationToken)
        {
          
            var state = await _stateRepository.GetByIdAsync(request.Id);
            if (state == null)
            {
                return Result<StateDto>.Failure($"Country with ID {request.Id} not found.");
            }
            
            var stateDto = _mapper.Map<StateDto>(state);
            return Result<StateDto>.Success(stateDto);
        }
    }
}