using Core.Application.Entity.Queries.GetEntity;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Data;
using Core.Application.Common.Interfaces.IEntity;
using AutoMapper;
using Core.Application.Common;
using Core.Domain.Events;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Microsoft.Extensions.Logging;

namespace Core.Application.Entity.Queries.GetEntityById
{
    public class GetEntityByIdQueryHandler : IRequestHandler<GetEntityByIdQuery, ApiResponseDTO<List<EntityDto>>>
    {
        private readonly IEntityQueryRepository _entityRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<GetEntityByIdQueryHandler> _logger;

    public GetEntityByIdQueryHandler(IEntityQueryRepository entityRepository,  IMapper mapper, IMediator mediator,ILogger<GetEntityByIdQueryHandler> logger)
    {
           _entityRepository = entityRepository;
           _mapper =mapper;
           _mediator = mediator;
           _logger = logger?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApiResponseDTO<List<EntityDto>>>  Handle(GetEntityByIdQuery request, CancellationToken cancellationToken)
    {
                _logger.LogInformation("Fetching Entity Request started: {request}", request.EntityId);
                var entitylist = await _entityRepository.GetByIdAsync(request.EntityId);

                if (entitylist == null || !entitylist.Any())
                {
                     _logger.LogWarning("No Entity Record {Entity} not found in DB.", request.EntityId);
                     return new ApiResponseDTO<List<EntityDto>>
                     {
                         IsSuccess = false,
                         Message = "Entity not found"
                     };
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
                _logger.LogInformation("Entity {Entities} Listed successfully.", entityDto.Count);
                return new ApiResponseDTO<List<EntityDto>>
                {
                    IsSuccess = true,
                    Message = "Entity Fetched Successfully",
                    Data = entityDto
                };
      
 
     }

    }
}