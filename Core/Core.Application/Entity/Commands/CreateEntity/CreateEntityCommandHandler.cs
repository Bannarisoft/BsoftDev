using Core.Application.Entity.Queries.GetEntity;
using Core.Application.Entity.Queries.GetEntityLastCode;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Entity.Commands.CreateEntity
{
    public class CreateEntityCommandHandler :  IRequestHandler<CreateEntityCommand, EntityDto>
    {
        private readonly IEntityRepository _IentityRepository;
        private readonly IMapper _Imapper;
        private readonly IMediator _Imediator;


         public CreateEntityCommandHandler(IEntityRepository Ientityrepository, IMapper Imapper,IMediator Imediator)
        {
            _IentityRepository = Ientityrepository;
            _Imapper = Imapper;
            _Imediator=Imediator;
        }

       public async Task<EntityDto> Handle(CreateEntityCommand request, CancellationToken cancellationToken)
        {
            var entityCode = await _Imediator.Send(new GetEntityLastCodeQuery(), cancellationToken);
            var entity = _Imapper.Map<Core.Domain.Entities.Entity>(request);
            entity.EntityCode = entityCode;
            await _IentityRepository.CreateAsync(entity);
            var entityDto = _Imapper.Map<EntityDto>(entity);
            return entityDto;
        }
    }
}