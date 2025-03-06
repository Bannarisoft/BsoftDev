using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepreciationGroup;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroup;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.DepreciationGroup.Commands.CreateDepreciationGroup
{
    public class CreateDepreciationGroupCommandHandler : IRequestHandler<CreateDepreciationGroupCommand, ApiResponseDTO<DepreciationGroupDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IDepreciationGroupCommandRepository _depreciationGroupRepository;
        private readonly IMediator _mediator;

        public CreateDepreciationGroupCommandHandler(IMapper mapper, IDepreciationGroupCommandRepository depreciationGroupRepository, IMediator mediator)
        {
            _mapper = mapper;
            _depreciationGroupRepository = depreciationGroupRepository;
            _mediator = mediator;    
        } 

        public async Task<ApiResponseDTO<DepreciationGroupDTO>> Handle(CreateDepreciationGroupCommand request, CancellationToken cancellationToken)
        {
            var depreciationGroupsExists = await _depreciationGroupRepository.ExistsByCodeAsync(request.Code??string.Empty);
            if (depreciationGroupsExists)
            {
                return new ApiResponseDTO<DepreciationGroupDTO> {
                    IsSuccess = false, 
                    Message = "DepreciationGroup Code already exists."
                };                 
            }

            // ✅ Check if AssetGroupId exists before creating the record
            var assetGroupExists = await _depreciationGroupRepository.ExistsByAssetGroupIdAsync(request.AssetGroupId);
            if (!assetGroupExists)
            {
                return new ApiResponseDTO<DepreciationGroupDTO>
                {
                    IsSuccess = false,
                    Message = $"AssetGroupId {request.AssetGroupId} does not exist."
                };
            }
            
            var depreciationGroupEntity = _mapper.Map<DepreciationGroups>(request);            
            var result = await _depreciationGroupRepository.CreateAsync(depreciationGroupEntity);
            
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: depreciationGroupEntity.Code ?? string.Empty,
                actionName: depreciationGroupEntity.DepreciationGroupName ?? string.Empty,
                details: $"DepreciationGroup '{depreciationGroupEntity.DepreciationGroupName}' was created. Code: {depreciationGroupEntity.Code}",
                module:"DepreciationGroup"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            var depreciationGroupDTO = _mapper.Map<DepreciationGroupDTO>(result);
            if (depreciationGroupDTO.Id > 0)
            {
                return new ApiResponseDTO<DepreciationGroupDTO>{
                    IsSuccess = true, 
                    Message = "DepreciationGroup created successfully.",
                    Data = depreciationGroupDTO
                };
            }
            return  new ApiResponseDTO<DepreciationGroupDTO>{
                IsSuccess = false, 
                Message = "DepreciationGroup not created."
            };      
        }
    }
}