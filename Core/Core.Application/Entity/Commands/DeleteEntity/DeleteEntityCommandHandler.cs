using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IEntity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Entity.Commands.DeleteEntity
{
    public class DeleteEntityCommandHandler  : IRequestHandler<DeleteEntityCommand, int>
    {
        private readonly IEntityCommandRepository _ientityRepository;
        private readonly IMapper _Imapper;
        private readonly ILogger<DeleteEntityCommandHandler> _Ilogger;

        public DeleteEntityCommandHandler(IEntityCommandRepository Ientityrepository,IMapper Imapper,ILogger<DeleteEntityCommandHandler> Ilogger)
        {
            _ientityRepository = Ientityrepository;
            _Imapper = Imapper;
            _Ilogger = Ilogger;
            
        }
        public async Task<int> Handle(DeleteEntityCommand request, CancellationToken cancellationToken)
        {       
        try
        {
        // Map the command to the Entity
        var entity = _Imapper.Map<Core.Domain.Entities.Entity>(request);

        // Call repository to delete the entity
        var result = await _ientityRepository.DeleteEntityAsync(request.EntityId, entity);

        if (result == -1) // Entity not found
        {
            throw new CustomException(
                "Entity not found",
                new[] { $"The entity with ID {request.EntityId} does not exist." },
                CustomException.HttpStatus.NotFound
            );
        }

        return result; // Return the number of affected rows (e.g., 1 for success)
    }
        catch (CustomException ex)
        {
        _Ilogger.LogWarning(ex, $"CustomException: {ex.Message}");
        throw; // Re-throw custom exceptions
        }
        catch (Exception ex)
        {
        _Ilogger.LogError(ex, "Unexpected error occurred while deleting the entity.");
        throw new Exception("An unexpected error occurred while deleting the entity.", ex);
        }
}

         
    }
}