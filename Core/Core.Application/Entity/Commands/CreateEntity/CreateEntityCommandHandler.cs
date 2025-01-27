using Core.Application.Entity.Queries.GetEntity;
using Core.Application.Entity.Queries.GetEntityLastCode;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Core.Application.Common.Interfaces.IEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Core.Application.Common;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;
using Serilog;



namespace Core.Application.Entity.Commands.CreateEntity
{
    public class CreateEntityCommandHandler :  IRequestHandler<CreateEntityCommand, ApiResponseDTO<int>>
    {
        private readonly IEntityCommandRepository _IentityRepository;
        private readonly IMapper _Imapper;
        private readonly IMediator _Imediator;

         private readonly ILogger<CreateEntityCommandHandler> _logger;


         public CreateEntityCommandHandler(IEntityCommandRepository Ientityrepository, IMapper Imapper,IMediator Imediator,ILogger<CreateEntityCommandHandler> logger)
        {
            _IentityRepository = Ientityrepository;
            _Imapper = Imapper;
            _Imediator=Imediator;
             _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

  public async Task<ApiResponseDTO<int>> Handle(CreateEntityCommand request, CancellationToken cancellationToken)
{
        _logger.LogInformation("Starting creation process for EntityCode: {Entitycode}", request);
        var entityCode = await _Imediator.Send(new GetEntityLastCodeQuery(), cancellationToken);
        _logger.LogInformation("Completed creation process for EntityCode: {Entitycode}", entityCode.Data);

        if (entityCode.Data == null || string.IsNullOrEmpty(entityCode.Data))
        { 
            _logger.LogError("Failed to create user for EntityCode: {EntityCode}", entityCode.Data);
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
        _logger.LogInformation("Starting Entity creation process for EntityCode: {EntityCode}", entity.EntityCode);


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
            _logger.LogInformation("Entity creation process completed for EntityCode: {EntityCode}", entity.EntityCode);
            var entityDto = _Imapper.Map<EntityDto>(entity);

             if (result > 0)
                  {
                     _logger.LogInformation("Entity {Entity} created successfully", result);
                        return new ApiResponseDTO<int>
                       {
                           IsSuccess = true,
                           Message = "Entity created successfully",
                           Data = result
                      };
                 }
            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "Entity Creation Failed",
                Data = result
            };
           

      

}

}
}