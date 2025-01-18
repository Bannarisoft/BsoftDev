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
        private readonly ILogger<DeleteEntityCommandHandler> _Ilogger;

        private readonly IMediator _mediator; 

        public DeleteEntityCommandHandler(IEntityCommandRepository Ientityrepository,IMapper Imapper,ILogger<DeleteEntityCommandHandler> Ilogger,IMediator mediator)
        {
            _ientityRepository = Ientityrepository;
            _Imapper = Imapper;
            _Ilogger = Ilogger;
            _mediator = mediator;
            
        }
        public async Task<ApiResponseDTO<int>> Handle(DeleteEntityCommand request, CancellationToken cancellationToken)
        {       
       
        
        var entity = _Imapper.Map<Core.Domain.Entities.Entity>(request);

        var result = await _ientityRepository.DeleteEntityAsync(request.EntityId, entity);

        if (result == -1) 
        {
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

         return new ApiResponseDTO<int>
         {
             IsSuccess = true,
             Message = "Success",
             Data = result
         };
   
}
         
   }
}