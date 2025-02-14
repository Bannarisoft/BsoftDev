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
    public class UpdateDepreciationGroupCommandHandler : IRequestHandler<UpdateDepreciationGroupCommand, ApiResponseDTO<DepreciationGroupDTO>>
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

        public async Task<ApiResponseDTO<DepreciationGroupDTO>> Handle(UpdateDepreciationGroupCommand request, CancellationToken cancellationToken)
        {
            var depreciationGroups = await _depreciationGroupQueryRepository.GetByIdAsync(request.Id);
            if (depreciationGroups is null)
            return new ApiResponseDTO<DepreciationGroupDTO>
            {
                IsSuccess = false,
                Message = "Invalid DepreciationGroupID. The specified Name does not exist or is inactive."
            };
            var oldDepreciationName = depreciationGroups.DepreciationGroupName;
            depreciationGroups.DepreciationGroupName = request.DepreciationGroupName;

            if (depreciationGroups is null || depreciationGroups.IsDeleted is BaseEntity.IsDelete.Deleted )
            {
                return new ApiResponseDTO<DepreciationGroupDTO>
                {
                    IsSuccess = false,
                    Message = "Invalid DepreciationGroupID. The specified Name does not exist or is deleted."
                };
            }
            if (depreciationGroups.IsActive != request.IsActive)
            {    
                 depreciationGroups.IsActive =  (BaseEntity.Status)request.IsActive;             
                await _depreciationGroupRepository.UpdateAsync(depreciationGroups.Id, depreciationGroups);
                if (request.IsActive is 0)
                {
                    return new ApiResponseDTO<DepreciationGroupDTO>
                    {
                        IsSuccess = false,
                        Message = "Code DeActivated."
                    };
                }
                else{
                    return new ApiResponseDTO<DepreciationGroupDTO>
                    {
                        IsSuccess = false,
                        Message = "Code Activated."
                    }; 
                }                                     
            }

            var depreciationGroupsExistsByName = await _depreciationGroupRepository.ExistsByCodeAsync(request.Code??string.Empty);
            if (depreciationGroupsExistsByName)
            {                                   
                return new ApiResponseDTO<DepreciationGroupDTO>
                {
                    IsSuccess = false,
                    Message = $"Code already exists and is {(BaseEntity.Status) request.IsActive}."
                };                     
            }
            var updatedDepreciationEntity = _mapper.Map<DepreciationGroups>(request);                   
            var updateResult = await _depreciationGroupRepository.UpdateAsync(request.Id, updatedDepreciationEntity);            

            var updatedDepreciationGroup =  await _depreciationGroupQueryRepository.GetByIdAsync(request.Id);    
            if (updatedDepreciationGroup != null)
            {
                var depreciationGroupDto = _mapper.Map<DepreciationGroupDTO>(updatedDepreciationGroup);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: depreciationGroupDto.Code ?? string.Empty,
                    actionName: depreciationGroupDto.DepreciationGroupName ?? string.Empty,                            
                    details: $"DepreciationGroup '{oldDepreciationName}' was updated to '{depreciationGroupDto.DepreciationGroupName}'.  Code: {depreciationGroupDto.Code}",
                    module:"DepreciationGroup"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);
                if(updateResult>0)
                {
                    return new ApiResponseDTO<DepreciationGroupDTO>
                    {
                        IsSuccess = true,
                        Message = "DepreciationGroup updated successfully.",
                        Data = depreciationGroupDto
                    };
                }
                return new ApiResponseDTO<DepreciationGroupDTO>
                {
                    IsSuccess = false,
                    Message = "DepreciationGroup not updated."
                };                
            }
            else
            {
                return new ApiResponseDTO<DepreciationGroupDTO>{
                    IsSuccess = false,
                    Message = "DepreciationGroup not found."
                };
            }
        }
    }
}