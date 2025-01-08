using MediatR;
using Core.Application.State.Queries.GetStates;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces.IState;

namespace Core.Application.State.Queries.GetStateAutoComplete
{
    public class GetStateAutoCompleteQueryHandler : IRequestHandler<GetStateAutoCompleteQuery, Result<List<StateDto>>>
    
    {
        private readonly IStateQueryRepository _stateRepository;
        private readonly IMapper _mapper;
        public GetStateAutoCompleteQueryHandler(IStateQueryRepository stateRepository,  IMapper mapper)
        {
            _stateRepository =stateRepository;
            _mapper =mapper;
        }

        public async Task<Result<List<StateDto>>> Handle(GetStateAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var result = await _stateRepository.GetByStateNameAsync(request.SearchPattern);
            if (!result.IsSuccess || result.Data == null || !result.Data.Any())
            {
                return Result<List<StateDto>>.Failure("No Cities found matching the search pattern.");
            }
            var stateDto = _mapper.Map<List<StateDto>>(result.Data);
            return Result<List<StateDto>>.Success(stateDto);         
        }
    }
    
}