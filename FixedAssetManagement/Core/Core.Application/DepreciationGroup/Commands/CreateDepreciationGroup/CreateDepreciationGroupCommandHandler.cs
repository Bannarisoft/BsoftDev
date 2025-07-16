using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces.IDepreciationGroup;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroup;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;


namespace Core.Application.DepreciationGroup.Commands.CreateDepreciationGroup
{
    public class CreateDepreciationGroupCommandHandler : IRequestHandler<CreateDepreciationGroupCommand, DepreciationGroupDTO>
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

        public async Task<DepreciationGroupDTO> Handle(CreateDepreciationGroupCommand request, CancellationToken cancellationToken)
        {
            if (await _depreciationGroupRepository.ExistsByCodeAsync(request.Code ?? string.Empty))
             throw new EntityAlreadyExistsException("DepreciationGroup", "Code", request.Code ?? "");

            var isDuplicate = await _depreciationGroupRepository.CheckForDuplicatesAsync(
            request.AssetGroupId, request.DepreciationMethod, request.BookType, 0);

            if (isDuplicate != null)
            {
                if (isDuplicate.IsActive == BaseEntity.Status.Active)
                    throw new EntityAlreadyExistsException("DepreciationGroup", "Combination", "Already exists.");
                else
                    throw new EntityAlreadyExistsException("DepreciationGroup", "Combination", "Already exists but status is inactive.");
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
            
            var dto  = _mapper.Map<DepreciationGroupDTO>(result);
            return dto.Id > 0 ? dto : throw new ExceptionRules("Failed to create DepreciationGroup.");
        }
    }
}