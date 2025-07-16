using AutoMapper;
using Contracts.Interfaces.External.IFixedAssetManagement;
using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IState;
using Core.Application.State.Queries.GetStates;
using Core.Domain.Entities;
using Core.Domain.Enums.Common;
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
         private readonly IFixedAssetStateValidationGrpcClient _fixedAssetStateValidationGrpcClient;


        public DeleteStateCommandHandler(IStateCommandRepository stateRepository, IMapper mapper, IStateQueryRepository stateQueryRepository, IMediator mediator, IFixedAssetStateValidationGrpcClient fixedAssetState)
        {
            _stateRepository = stateRepository;
            _mapper = mapper;
            _stateQueryRepository = stateQueryRepository;
            _mediator = mediator;
            _fixedAssetStateValidationGrpcClient = fixedAssetState;
        }

        public async Task<ApiResponseDTO<StateDto>> Handle(DeleteStateCommand request, CancellationToken cancellationToken)
        {

             bool iscountryUsedInFixedAsset = await _fixedAssetStateValidationGrpcClient.CheckIfStateIsUsedForFixedAssetAsync(request.Id);  
             if (iscountryUsedInFixedAsset)
            {
                return new ApiResponseDTO<StateDto>
                {
                    IsSuccess = false,
                    Message = "Cannot delete State. It is still in use in FixedAsset system."
                };
            }

            var state = await _stateQueryRepository.GetByIdAsync(request.Id);
            if (state is null || state.IsDeleted is  Enums.IsDelete.Deleted)
            {
                 return new ApiResponseDTO<StateDto>
                {
                    IsSuccess = false,
                    Message = "Invalid StateID. The specified State does not exist or is inactive."
                };
            }        
            var city = await _stateQueryRepository.GetCityByStateAsync(request.Id);            
            if (city.Count>0)
            {                
                 return new ApiResponseDTO<StateDto>
                {
                    IsSuccess = false,
                    Message = "City already exists for this State.Cannot delete the State."
                };
            }    
            var stateDelete = _mapper.Map<States>(request); 
            var updateResult = await _stateRepository.DeleteAsync(request.Id, stateDelete);
            if (updateResult > 0)
            {
                var stateDto = _mapper.Map<StateDto>(stateDelete);  
                //Domain Event   
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: stateDto.StateCode ?? string.Empty,
                    actionName: stateDto.StateName ?? string.Empty,
                    details: $"State '{stateDto.StateName}' was created. StateCode: {stateDto.StateCode}",
                    module:"State"
                );               
                await _mediator.Publish(domainEvent, cancellationToken);          
                return new ApiResponseDTO<StateDto>
                {
                    IsSuccess = true,
                    Message = "State deleted successfully.",
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