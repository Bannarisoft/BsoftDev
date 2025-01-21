using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IState;
using Core.Application.State.Queries.GetStates;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.State.Commands.DeleteState
{
    public class DeleteStateCommandHandler : IRequestHandler<DeleteStateCommand, ApiResponseDTO<StateDto>>
    {
        private readonly IStateCommandRepository _stateRepository;
        private readonly IStateQueryRepository _stateQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public DeleteStateCommandHandler(IStateCommandRepository stateRepository, IMapper mapper,IStateQueryRepository stateQueryRepository, IMediator mediator)
        {
            _stateRepository = stateRepository;
            _mapper = mapper;
            _stateQueryRepository = stateQueryRepository;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<StateDto>> Handle(DeleteStateCommand request, CancellationToken cancellationToken)
        {
            var state = await _stateQueryRepository.GetByIdAsync(request.Id);
            if (state == null || state.IsActive != 1)
            {
                 return new ApiResponseDTO<StateDto>
                {
                    IsSuccess = false,
                    Message = "Invalid StateID. The specified State does not exist or is inactive."
                };
            }            
            var stateUpdate = new States
            {
                Id = request.Id,
                StateCode = state.StateCode,
                StateName = state.StateName,
                CountryId = state.CountryId,
                IsActive = 0
            };            
            var updateResult = await _stateRepository.DeleteAsync(request.Id, stateUpdate);
            if (updateResult > 0)
            {
                var stateDto = _mapper.Map<StateDto>(stateUpdate);  
                //Domain Event   
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: stateDto.StateCode,
                    actionName: stateDto.StateName,
                    details: $"State '{stateDto.StateName}' was created. StateCode: {stateDto.StateCode}",
                    module:"State"
                );               
                await _mediator.Publish(domainEvent, cancellationToken);          
                return new ApiResponseDTO<StateDto>
                {
                    IsSuccess = true,
                    Message = "City deleted successfully.",
                    Data = stateDto
                };
            }
            return new ApiResponseDTO<StateDto>
            {
                IsSuccess = false,
                Message = "State deletion failed."                
            };        
        }
    }
}