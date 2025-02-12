using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepreciationGroup;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroup;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Enums.Common;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.DepreciationGroup.Commands.DeleteDepreciationGroup
{
    public class DeleteDepreciationGroupCommandHandler : IRequestHandler<DeleteDepreciationGroupCommand, ApiResponseDTO<DepreciationGroupDTO>>
    {
        private readonly IDepreciationGroupCommandRepository _depreciationGroupRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly IDepreciationGroupQueryRepository _depreciationGroupQueryRepository;
        
        public DeleteDepreciationGroupCommandHandler(IDepreciationGroupCommandRepository depreciationGroupRepository, IMapper mapper,  IMediator mediator,IDepreciationGroupQueryRepository depreciationGroupQueryRepository)
        {
            _depreciationGroupRepository = depreciationGroupRepository;
             _mapper = mapper;        
            _mediator = mediator;
            _depreciationGroupQueryRepository=depreciationGroupQueryRepository;
        }

        public async Task<ApiResponseDTO<DepreciationGroupDTO>> Handle(DeleteDepreciationGroupCommand request, CancellationToken cancellationToken)
        {
             // Fetch the city to be deleted
            var depreciationGroups = await _depreciationGroupQueryRepository.GetByIdAsync(request.Id);
            if (depreciationGroups is null || depreciationGroups.IsDeleted is BaseEntity.IsDelete.Deleted )
            {
                return new ApiResponseDTO<DepreciationGroupDTO>
                {
                    IsSuccess = false,
                    Message = "Invalid DepreciationGroupID. The specified GroupName does not exist or is inactive."
                };
            }
            var depreciationGroupsDelete = _mapper.Map<DepreciationGroups>(request);      
            var updateResult = await _depreciationGroupRepository.DeleteAsync(request.Id, depreciationGroupsDelete);
            if (updateResult > 0)
            {
                var depreciationGroupDto = _mapper.Map<DepreciationGroupDTO>(depreciationGroupsDelete);  
                //Domain Event  
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: depreciationGroupsDelete.Code ?? string.Empty,
                    actionName: depreciationGroupsDelete.DepreciationGroupName ?? string.Empty,
                    details: $"DepreciationGroup '{depreciationGroupDto.DepreciationGroupName}' was created. Code: {depreciationGroupDto.Code}",
                    module:"DepreciationGroup"
                );               
                await _mediator.Publish(domainEvent, cancellationToken);                 
                return new ApiResponseDTO<DepreciationGroupDTO>
                {
                    IsSuccess = true,
                    Message = "DepreciationGroup deleted successfully.",
                    Data = depreciationGroupDto
                };
            }

            return new ApiResponseDTO<DepreciationGroupDTO>
            {
                IsSuccess = false,
                Message = "DepreciationGroup deletion failed."                             
            };
           
        }
    }
}