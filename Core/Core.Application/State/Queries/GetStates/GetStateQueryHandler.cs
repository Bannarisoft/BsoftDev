using AutoMapper;
using Core.Application.Common.Interfaces.IState;
using Core.Application.State.Queries.GetStates;
using MediatR;

namespace Core.Application.State.Queries.GetCountries
{
    public class GetStateQueryHandler : IRequestHandler<GetStateQuery, List<StateDto>>
    {
        private readonly IStateQueryRepository _stateRepository;
        private readonly IMapper _mapper;

        public GetStateQueryHandler(IStateQueryRepository stateRepository , IMapper mapper)
        {
           _stateRepository = stateRepository;
            _mapper = mapper;
        }
        public async Task<List<StateDto>> Handle(GetStateQuery request, CancellationToken cancellationToken)
        {
            var states = await _stateRepository.GetAllStatesAsync();
            var statesList = _mapper.Map<List<StateDto>>(states);
            return statesList;
        }
    }
}