using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepreciationGroup;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroup;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.DepreciationGroup.Commands.DeleteDepreciationGroup
{
    public class DeleteDepreciationGroupCommandHandler : IRequestHandler<DeleteDepreciationGroupCommand, ApiResponseDTO<DepreciationGroupDTO>>
    {
        private readonly IDepreciationGroupCommandRepository _depreciationRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly IDepreciationGroupQueryRepository _depreciationQueryRepository;
        
        public DeleteDepreciationGroupCommandHandler(IDepreciationGroupCommandRepository depreciationRepository, IMapper mapper,  IMediator mediator,IDepreciationGroupQueryRepository depreciationQueryRepository)
        {
            _depreciationRepository = depreciationRepository;
            _mapper = mapper;        
            _mediator = mediator;
            _depreciationQueryRepository=depreciationQueryRepository;
        }

        public async Task<ApiResponseDTO<DepreciationGroupDTO>> Handle(DeleteDepreciationGroupCommand request, CancellationToken cancellationToken)
        {             
            var depreciationGroups = await _depreciationQueryRepository.GetByIdAsync(request.Id);
            if (depreciationGroups is null )
            {
                return new ApiResponseDTO<DepreciationGroupDTO>
                {
                    IsSuccess = false,
                    Message = "Invalid DepreciationGroupID."
                };
            }
            var depreciationDelete = _mapper.Map<DepreciationGroups>(request);      
            var updateResult = await _depreciationRepository.DeleteAsync(request.Id, depreciationDelete);
            if (updateResult > 0)
            {
                var depreciationGroupDto = _mapper.Map<DepreciationGroupDTO>(depreciationDelete);  
                //Domain Event  
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: depreciationDelete.Code ?? string.Empty,
                    actionName: depreciationDelete.DepreciationGroupName ?? string.Empty,
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