using MediatR;
using Core.Application.State.Queries.GetStates;
using AutoMapper;
using Core.Application.Common.Interfaces.IState;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;

namespace Core.Application.State.Queries.GetStateByCountryId
{
    public class GetStateByCountryIdQueryHandler : IRequestHandler<GetStateByCountryIdQuery, ApiResponseDTO<List<StateDto>>>    
    {
        private readonly IStateQueryRepository _stateRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        public GetStateByCountryIdQueryHandler(IStateQueryRepository stateRepository, IMapper mapper, IMediator mediator)
        {
            _stateRepository = stateRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<StateDto>>> Handle(GetStateByCountryIdQuery request, CancellationToken cancellationToken)
        {            
            var state = await _stateRepository.GetStateByCountryIdAsync(request.Id);            
            if (state is null || !state.Any())
            {                
                 return new ApiResponseDTO<List<StateDto>>
                {
                    IsSuccess = false,
                    Message = "No States found matching the search pattern."
                };
            }                        
             var stateDto = _mapper.Map<List<StateDto>>(state); 
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetStateByCountryId",
                actionCode:"" ,        
                actionName: "",                
                details: $"Get State by CountryId: {request.Id}. details was fetched.",
                module:"State"
            );
            await _mediator.Publish(domainEvent, cancellationToken);            
            return new ApiResponseDTO<List<StateDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = stateDto
            };   
        }
    }
}
