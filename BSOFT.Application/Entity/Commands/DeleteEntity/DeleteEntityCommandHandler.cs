using BSOFT.Application.Common.Interfaces;
using BSOFT.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BSOFT.Application.Entity.Commands.DeleteEntity
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
            var Updateentity = new BSOFT.Domain.Entities.Entity()
            {
                EntityId = request.EntityId,
                IsActive = request.IsActive 
            };
            return await _entityRepository.DeleteAsync(request.EntityId,Updateentity);
        }
    }
}