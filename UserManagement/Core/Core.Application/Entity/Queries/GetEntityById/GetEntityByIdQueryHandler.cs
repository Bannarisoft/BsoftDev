using Core.Application.Entity.Queries.GetEntity;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Data;
using Core.Application.Common.Interfaces.IEntity;
using AutoMapper;
using Core.Application.Common;
using Core.Domain.Events;

using Core.Application.Common.HttpResponse;
using Microsoft.Extensions.Logging;

namespace Core.Application.Entity.Queries.GetEntityById
{
    public class GetEntityByIdQueryHandler : IRequestHandler<GetEntityByIdQuery, ApiResponseDTO<List<GetEntityDTO>>>
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

    public async Task<ApiResponseDTO<List<GetEntityDTO>>>  Handle(GetEntityByIdQuery request, CancellationToken cancellationToken)
    {
                _logger.LogInformation($"Fetching Entity Request started: {request.EntityId}");
                var entitylist = await _entityRepository.GetByIdAsync(request.EntityId);

                if (entitylist is null || !entitylist.Any())
                {
                     _logger.LogWarning($"No Entity Record {request.EntityId} not found in DB.");
                     return new ApiResponseDTO<List<GetEntityDTO>>
                     {
                         IsSuccess = false,
                         Message = "Entity not found"
                     };
                }
                var entityDto = _mapper.Map<List<GetEntityDTO>>(entitylist);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetEntityByIdQuery",
                    actionCode: entityDto[0].EntityCode,      
                    actionName: entityDto[0].EntityName,              
                    details: $"Entity '{entityDto[0].EntityName}' was Fetched. EntityCode: {entityDto[0].EntityCode}",
                    module:"Entity"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
                _logger.LogInformation($"Entity {entityDto.Count} Listed successfully.");
                return new ApiResponseDTO<List<GetEntityDTO>>
                {
                    IsSuccess = true,
                    Message = "Entity Fetched Successfully",
                    Data = entityDto
                };
      
 
     }

    }
}