using Core.Application.Entity.Queries.GetEntity;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Data;
using Core.Application.Common.Interfaces.IEntity;
using AutoMapper;

namespace Core.Application.Entity.Queries.GetEntityById
{
    public class GetEntityByIdQueryHandler : IRequestHandler<GetEntityByIdQuery, EntityDto?>
    {
     private readonly IEntityQueryRepository _entityRepository;        
        private readonly IMapper _mapper;

    public GetEntityByIdQueryHandler(IEntityQueryRepository entityRepository,  IMapper mapper)
    {
           _entityRepository = entityRepository;
         _mapper =mapper;
    }

    public async Task<EntityDto?> Handle(GetEntityByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _entityRepository.GetByIdAsync(request.EntityId);
          return _mapper.Map<EntityDto>(result);
    }

    }
}