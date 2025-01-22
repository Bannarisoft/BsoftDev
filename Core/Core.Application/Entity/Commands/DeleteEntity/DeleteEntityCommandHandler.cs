using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IEntity;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Entity.Commands.DeleteEntity
{
    public class DeleteEntityCommandHandler  : IRequestHandler<DeleteEntityCommand,  ApiResponseDTO<int>>
    {
        private readonly IEntityCommandRepository _ientityRepository;
        private readonly IMapper _Imapper;
        private readonly ILogger<DeleteEntityCommandHandler> _logger;

        private readonly IMediator _mediator; 

        public DeleteEntityCommandHandler(IEntityCommandRepository Ientityrepository,IMapper Imapper,ILogger<DeleteEntityCommandHandler> logger,IMediator mediator)
        {
            _ientityRepository = Ientityrepository;
            _Imapper = Imapper;
            _logger = logger?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator;
            
        }
        public async Task<ApiResponseDTO<int>> Handle(DeleteEntityCommand request, CancellationToken cancellationToken)
        {       
       
       _logger.LogInformation("Starting Entity Deletion process for EntityId: {EntityId}", request.EntityId);
        
        var entity = _Imapper.Map<Core.Domain.Entities.Entity>(request.UpdateEntityStatusDto);

        var result = await _ientityRepository.DeleteEntityAsync(request.EntityId, entity);

        if (result == -1) 
        {
            _logger.LogInformation("Entity {EntityId} not found.", request.EntityId);
            return new ApiResponseDTO<int> { IsSuccess = false, Message = "Entity not found."};
        }
        //Domain Event
        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "Delete",
            actionCode: entity.Id.ToString(),
            actionName:"",
            details:$"EntityCode: {request.EntityId} was Changed to Status Inactive.",
            module:"Entity"
        );            
        await _mediator.Publish(domainEvent, cancellationToken);
        _logger.LogInformation("Successfully completed Entity Deletion process for EntityId: {EntityId}", request.EntityId);
         return new ApiResponseDTO<int>
         {
             IsSuccess = true,
             Message = "Success",
             Data = result
         };
   
}
         
   }
}