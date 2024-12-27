using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Entity.Commands.DeleteEntity
{
    public class DeleteEntityCommandHandler  : IRequestHandler<DeleteEntityCommand, int>
    {
        private readonly IEntityRepository _entityRepository;

        public DeleteEntityCommandHandler(IEntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
            
        }
        public async Task<int> Handle(DeleteEntityCommand request, CancellationToken cancellationToken)
        {
            var Updateentity = new Core.Domain.Entities.Entity()
            {
                EntityId = request.EntityId,
                IsActive = request.IsActive 
            };
            return await _entityRepository.DeleteAsync(request.EntityId,Updateentity);
        }
    }
}