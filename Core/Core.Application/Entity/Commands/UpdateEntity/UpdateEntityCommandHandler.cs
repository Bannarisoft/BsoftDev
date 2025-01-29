using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IEntity;
using Core.Application.Entity.Queries.GetEntity;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Entity.Commands.UpdateEntity
{
    public class UpdateEntityCommandHandler : IRequestHandler<UpdateEntityCommand, ApiResponseDTO<int>>
    {
       private readonly IEntityCommandRepository _Ientityrepository;
        private readonly IMapper _Imapper;
        private readonly ILogger<UpdateEntityCommandHandler> _logger;
        private readonly IMediator _mediator; 
       public UpdateEntityCommandHandler(IEntityCommandRepository Ientityrepository,IMapper Imapper, ILogger<UpdateEntityCommandHandler> logger,IMediator mediator)
        {
            _Ientityrepository = Ientityrepository;
            _Imapper = Imapper;
            _logger = logger?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator;
             
        }

       public async Task<ApiResponseDTO<int>> Handle(UpdateEntityCommand request, CancellationToken cancellationToken)
        { 
            _logger.LogInformation("Starting Entity Update process for EntityId: {EntityId}", request.EntityId);
            var entity = _Imapper.Map<Core.Domain.Entities.Entity>(request);
            var result = await _Ientityrepository.UpdateAsync (request.EntityId, entity);

            if (result == -1) // Entity not found
            {
                _logger.LogInformation("Entity {EntityId} not found.", request.EntityId);
                return new ApiResponseDTO<int> { IsSuccess = false, Message = "Entity not found." };
            }

              //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "Update",
            actionCode: entity.Id.ToString(),
            actionName: entity.EntityName,                            
            details:$"Entity '{entity.EntityName}' was Updated. EntityCode: {request.EntityId}",
            module:"Entity"
            );            
            await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation("Successfully completed Entity Update process for EntityId: {EntityId}", request.EntityId);
            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "Success",
                Data = result
            };

        
        
        }
     }
}