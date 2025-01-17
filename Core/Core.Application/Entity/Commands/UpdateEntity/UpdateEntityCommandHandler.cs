using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IEntity;
using Core.Application.Entity.Queries.GetEntity;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Entity.Commands.UpdateEntity
{
    public class UpdateEntityCommandHandler : IRequestHandler<UpdateEntityCommand, Result<int>>
    {
       private readonly IEntityCommandRepository _Ientityrepository;
        private readonly IMapper _Imapper;
        private readonly ILogger<UpdateEntityCommandHandler> _ilogger;
        private readonly IMediator _mediator; 
       public UpdateEntityCommandHandler(IEntityCommandRepository Ientityrepository,IMapper Imapper, ILogger<UpdateEntityCommandHandler> Ilogger,IMediator mediator)
        {
            _Ientityrepository = Ientityrepository;
            _Imapper = Imapper;
            _ilogger = Ilogger;
            _mediator = mediator;
             
        }

       public async Task<Result<int>> Handle(UpdateEntityCommand request, CancellationToken cancellationToken)
        { 
        try
        {   
            var entity = _Imapper.Map<Core.Domain.Entities.Entity>(request);
            var result = await _Ientityrepository.UpdateAsync (request.EntityId, entity);

            if (result == -1) // Entity not found
            {
            throw new CustomException(
                "Entity not found",
                new[] { $"The entity with ID {request.EntityId} does not exist." },
          
            CustomException.HttpStatus.NotFound
            );
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
            return Result<int>.Success(result);
        }
        catch (CustomException ex)
        {
        _ilogger.LogWarning(ex, $"CustomException: {ex.Message}");
        throw; // Re-throw custom exceptions
        }
        catch (Exception ex)
        {
        _ilogger.LogError(ex, "Unexpected error occurred while deleting the entity.");
        throw new Exception("An unexpected error occurred while deleting the entity.", ex);
        }
        }
     }
}