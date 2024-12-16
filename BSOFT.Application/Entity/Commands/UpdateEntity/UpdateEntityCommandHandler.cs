using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BSOFT.Application.Entity.Commands.UpdateEntity
{
    public class UpdateEntityCommandHandler : IRequestHandler<UpdateEntityCommand, int>
    {
       private readonly IEntityRepository _entityRepository;
       public UpdateEntityCommandHandler(IEntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

         public async Task<int> Handle(UpdateEntityCommand request, CancellationToken cancellationToken)
        {
            var UpdateEntity = new BSOFT.Domain.Entities.Entity()
            {
                EntityId=request.EntityId,
                EntityName = request.EntityName,
                EntityDescription = request.EntityDescription,
                Address = request.Address,
                Phone = request.Phone,
                Email = request.Email,
                IsActive = request.IsActive,
                 
            };

            return await _entityRepository.UpdateAsync(request.EntityId, UpdateEntity);
        } 
    }
}