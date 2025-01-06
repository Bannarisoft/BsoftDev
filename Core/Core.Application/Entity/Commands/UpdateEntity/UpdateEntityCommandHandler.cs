using AutoMapper;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IEntity;
using Core.Application.Entity.Queries.GetEntity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Entity.Commands.UpdateEntity
{
    public class UpdateEntityCommandHandler : IRequestHandler<UpdateEntityCommand, EntityDto>
    {
       private readonly IEntityCommandRepository _Ientityrepository;
        private readonly IMapper _Imapper;
        private readonly ILogger<UpdateEntityCommandHandler> _logger;
       public UpdateEntityCommandHandler(IEntityCommandRepository Ientityrepository,IMapper Imapper, ILogger<UpdateEntityCommandHandler> Ilogger)
        {
            _Ientityrepository = Ientityrepository;
            _Imapper = Imapper;
            _logger = Ilogger;
        }

       public async Task<EntityDto> Handle(UpdateEntityCommand request, CancellationToken cancellationToken)
        {
            var entity = _Imapper.Map<Core.Domain.Entities.Entity>(request);
            await _Ientityrepository.UpdateAsync (request.EntityId, entity);
            var entityDto = _Imapper.Map<EntityDto>(entity);
            return entityDto;

        }
    }
}