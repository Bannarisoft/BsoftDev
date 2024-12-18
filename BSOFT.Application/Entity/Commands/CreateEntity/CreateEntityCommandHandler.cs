using BSOFT.Application.Entity.Queries.GetEntity;
using BSOFT.Application.Entity.Queries.GetEntityLastCode;
using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BSOFT.Application.Entity.Commands.CreateEntity
{
    public class CreateEntityCommandHandler :  IRequestHandler<CreateEntityCommand, EntityVm>
    {
        private readonly IEntityRepository _entityRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;


         public CreateEntityCommandHandler(IEntityRepository EntityRepository, IMapper mapper,IMediator mediator)
        {
            _entityRepository = EntityRepository;
            _mapper = mapper;
            _mediator=mediator;
        }

         public async Task<EntityVm> Handle(CreateEntityCommand request, CancellationToken cancellationToken)
        {
            var entityCode = await _mediator.Send(new GetEntityLastCodeQuery(), cancellationToken);
            var EntityReg = new BSOFT.Domain.Entities.Entity
            {
                EntityCode = entityCode,
                EntityName = request.EntityName,
                EntityDescription = request.EntityDescription,
                Address = request.Address,
                Phone = request.Phone,
                Email = request.Email,
                IsActive = request.IsActive      

            };

            var result = await _entityRepository.CreateAsync(EntityReg);
            return _mapper.Map<EntityVm>(result);
        }
    }
}