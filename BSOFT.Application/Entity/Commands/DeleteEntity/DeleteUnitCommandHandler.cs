using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BSOFT.Application.Entity.Commands.DeleteEntity
{
    public class DeleteUnitCommandHandler  : IRequestHandler<DeleteEntityCommand, int>
    {
        private readonly IEntityRepository _entityRepository;

        public DeleteUnitCommandHandler(IEntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
            
        }
        public async Task<int> Handle(DeleteEntityCommand request, CancellationToken cancellationToken)
        {
            return await _entityRepository.DeleteAsync(request.EntityId);
        }
    }
}