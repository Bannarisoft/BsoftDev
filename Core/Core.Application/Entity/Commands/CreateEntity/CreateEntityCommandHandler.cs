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


namespace Core.Application.Entity.Commands.CreateEntity
{
    public class CreateEntityCommandHandler :  IRequestHandler<CreateEntityCommand, Result<int>>
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

    //    public async Task<int> Handle(CreateEntityCommand request, CancellationToken cancellationToken)
    //     {
    //        var entityCode = await _Imediator.Send(new GetEntityLastCodeQuery(), cancellationToken);
    //        var entity = _Imapper.Map<Core.Domain.Entities.Entity>(request);
    //        entity.EntityCode = entityCode;
    //        var entityId = await _IentityRepository.CreateAsync(entity);
    //        return entityId;
    //     }

  public async Task<Result<int>> Handle(CreateEntityCommand request, CancellationToken cancellationToken)
{
    try
    {
        // Get the entity code by sending the GetEntityLastCodeQuery to the mediator
        var entityCode = await _Imediator.Send(new GetEntityLastCodeQuery(), cancellationToken);

        // If entityCode is not retrieved properly, throw a CustomException
        if (string.IsNullOrEmpty(entityCode))
        {
            throw new CustomException(
                "Entity code could not be generated.",
                new[] { "The system failed to generate a valid entity code." },
                CustomException.HttpStatus.InternalServerError
            );
        }

        // Map the request to the Core domain entity
        var entity = _Imapper.Map<Core.Domain.Entities.Entity>(request);
        entity.EntityCode = entityCode;

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
            return Result<int>.Success(result);
           

      
    }
    catch (CustomException ex)
    {
          throw new CustomException(
            "An unexpected error occurred while creating the entity.",
            new[] { ex.Message },
            CustomException.HttpStatus.InternalServerError
        );
    }
    catch (Exception ex)
    {
        // Log any unexpected exception as an error
       _ilogger.LogError(ex, "An unexpected error occurred while creating the entity.");
        // Throw a generic CustomException for unexpected errors
        throw new CustomException(
            "An unexpected error occurred while creating the entity.",
            new[] { ex.Message },
            CustomException.HttpStatus.InternalServerError
        );
    }
}

}
}