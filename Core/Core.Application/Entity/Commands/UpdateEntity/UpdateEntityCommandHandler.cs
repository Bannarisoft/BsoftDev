using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IEntity;
using Core.Application.Entity.Queries.GetEntity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Entity.Commands.UpdateEntity
{
    public class UpdateEntityCommandHandler : IRequestHandler<UpdateEntityCommand, int>
    {
       private readonly IEntityCommandRepository _Ientityrepository;
        private readonly IMapper _Imapper;
        private readonly ILogger<UpdateEntityCommandHandler> _ilogger;
       public UpdateEntityCommandHandler(IEntityCommandRepository Ientityrepository,IMapper Imapper, ILogger<UpdateEntityCommandHandler> Ilogger)
        {
            _Ientityrepository = Ientityrepository;
            _Imapper = Imapper;
             _ilogger = Ilogger;
             
        }

       public async Task<int> Handle(UpdateEntityCommand request, CancellationToken cancellationToken)
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

            return result; // Return the number of affected rows (e.g., 1 for success)

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