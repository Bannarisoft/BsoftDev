using Core.Application.Entity.Queries.GetEntity;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Data;
using Core.Application.Common.Interfaces.IEntity;
using AutoMapper;
using Core.Application.Common;
using Core.Domain.Events;
using Core.Application.Common.Exceptions;

namespace Core.Application.Entity.Queries.GetEntityById
{
    public class GetEntityByIdQueryHandler : IRequestHandler<GetEntityByIdQuery, Result<List<EntityDto>>>
    {
        private readonly IEntityQueryRepository _entityRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

    public GetEntityByIdQueryHandler(IEntityQueryRepository entityRepository,  IMapper mapper, IMediator mediator)
    {
           _entityRepository = entityRepository;
           _mapper =mapper;
           _mediator = mediator;
    }

    public async Task<Result<List<EntityDto>>>  Handle(GetEntityByIdQuery request, CancellationToken cancellationToken)
    {
 
                var entitylist = await _entityRepository.GetByIdAsync(request.EntityId);
                if (entitylist == null || !entitylist.Any())
                {
                return Result<List<EntityDto>>.Failure("Entity not found.");
                }
                var entityDto = _mapper.Map<List<EntityDto>>(entitylist);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetEntityByIdQuery",
                    actionCode: entityDto[0].EntityCode,      
                    actionName: entityDto[0].EntityName,              
                    details: $"Entity '{entityDto[0].EntityName}' was Fetched. EntityCode: {entityDto[0].EntityCode}",
                    module:"Entity"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
                return Result<List<EntityDto>>.Success(entityDto);
      
 
     }

    }
}