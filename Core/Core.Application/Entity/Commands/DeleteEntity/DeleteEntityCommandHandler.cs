using AutoMapper;
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
           var entity = _Imapper.Map<Core.Domain.Entities.Entity>(request.UpdateEntityStatusDto);
           await _ientityRepository.DeleteAsync(request.EntityId,entity);
           return entity.Id;
        }   
        catch (Exception ex)
        {
                // Log the exception
                _Ilogger.LogError(ex, "Error updating Entity");

                // Throw a custom exception 
                throw new Exception("Error updating Entity", ex);
        }
        }
    }
}