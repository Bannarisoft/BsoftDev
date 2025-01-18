using Core.Application.Entity.Queries.GetEntity;
using Core.Application.Entity.Queries.GetEntityLastCode;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Core.Application.Common.Interfaces.IEntity;
using Microsoft.EntityFrameworkCore;
using Core.Application.Common.Exceptions;
using Microsoft.Extensions.Logging;
using Core.Application.Common;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;


namespace Core.Application.Entity.Commands.CreateEntity
{
    public class CreateEntityCommandHandler :  IRequestHandler<CreateEntityCommand, ApiResponseDTO<int>>
    {
        private readonly IEntityCommandRepository _IentityRepository;
        private readonly IMapper _Imapper;
        private readonly IMediator _Imediator;

        private readonly ILogger<CreateEntityCommandHandler> _ilogger;


         public CreateEntityCommandHandler(IEntityCommandRepository Ientityrepository, IMapper Imapper,IMediator Imediator,ILogger<CreateEntityCommandHandler> Ilogger)
        {
            _IentityRepository = Ientityrepository;
            _Imapper = Imapper;
            _Imediator=Imediator;
            _ilogger = Ilogger;
        }

   

  public async Task<ApiResponseDTO<int>> Handle(CreateEntityCommand request, CancellationToken cancellationToken)
{
  
        var entityCode = await _Imediator.Send(new GetEntityLastCodeQuery(), cancellationToken);

       
        if (entityCode.Data == null || string.IsNullOrEmpty(entityCode.Data))
        {
            return new ApiResponseDTO<int> 
            { 
                IsSuccess = false, 
                Message = "Failed to generate entity code." 
                
            };
        }

        // Map the request to the Core domain entity
        var entity = _Imapper.Map<Core.Domain.Entities.Entity>(request);
        entity.EntityCode = entityCode.Data;

        // Log that the entity creation process is about to begin
        _ilogger.LogInformation($"Attempting to create a new entity with code {entityCode}.");


            var result = await _IentityRepository.CreateAsync(entity);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: entity.EntityCode,
                actionName: entity.EntityName,
                details: $"Entity '{entity.EntityName}' was created. EntityCode: {entity.Id}",
                module:"Entity"
            );
            await _Imediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "Entity Created Successfully",
                Data = result
            };
           

      

}

}
}