using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepreciationGroup;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroup;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.DepreciationGroup.Commands.UpdateDepreciationGroup
{
    public class UpdateDepreciationGroupCommandHandler : IRequestHandler<UpdateDepreciationGroupCommand, ApiResponseDTO<bool>>
    {
        private readonly IDepreciationGroupCommandRepository _depreciationGroupRepository;
        private readonly IDepreciationGroupQueryRepository _depreciationGroupQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public UpdateDepreciationGroupCommandHandler(IDepreciationGroupCommandRepository depreciationGroupRepository, IMapper mapper,IDepreciationGroupQueryRepository depreciationGroupQueryRepository, IMediator mediator)
        {
            _depreciationGroupRepository = depreciationGroupRepository;
            _mapper = mapper;
            _depreciationGroupQueryRepository = depreciationGroupQueryRepository;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<bool>> Handle(UpdateDepreciationGroupCommand request, CancellationToken cancellationToken)
        {
            var depreciationGroups = await _depreciationGroupQueryRepository.GetByIdAsync(request.Id);
            if (depreciationGroups is null)
            return new ApiResponseDTO<bool>
            {
                IsSuccess = false,
                Message = "Invalid DepreciationGroupID. The specified Name does not exist"
            };

              // Check for duplicate GroupName or SortOrder
            var (isNameDuplicate, isCodeDuplicate,isSortOrderDuplicate) = await _depreciationGroupRepository
                                    .CheckForDuplicatesAsync(request.DepreciationGroupName ?? string.Empty,request.Code ?? string.Empty, request.SortOrder, request.Id);

            if (isNameDuplicate || isCodeDuplicate || isSortOrderDuplicate)
            {
                string errorMessage = isNameDuplicate && isCodeDuplicate && isSortOrderDuplicate
                ? "Both Category Name and Sort Order already exist."
                : isNameDuplicate
                ? "DepreciationGroup with the same Name already exists."
                : isCodeDuplicate
                ? "DepreciationGroup with the same Code already exists."
                : "DepreciationGroup with the same Sort Order already exists.";

                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = errorMessage                    
                };
            }
            
            var oldDepreciationName = depreciationGroups.DepreciationGroupName;
            
            var updatedDepreciationEntity = _mapper.Map<DepreciationGroups>(request);                   
            var updateResult = await _depreciationGroupRepository.UpdateAsync(updatedDepreciationEntity);            

          
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: request.Code ?? string.Empty,
                    actionName: request.DepreciationGroupName ?? string.Empty,                            
                    details: $"DepreciationGroup '{oldDepreciationName}' was updated to '{request.DepreciationGroupName}'.  Code: {request.Code}",
                    module:"DepreciationGroup"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);
                if(updateResult)
                {
                    return new ApiResponseDTO<bool>{IsSuccess = true, Message = "Depreciation Groups updated successfully."};
                }
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = "DepreciationGroup not updated."
                };                
            }         
    }
}