using BSOFT.Application.Entity.Queries.GetEntity;
using BSOFT.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace BSOFT.Application.Entity.Queries.GetEntityById
{
    public class GetEntityByIdQueryHandler : IRequestHandler<GetEntityByIdQuery,EntityVm>
    {
    private readonly IEntityRepository _entityRepository;
     private readonly IMapper _mapper;

         public GetEntityByIdQueryHandler(IEntityRepository entityRepository, IMapper mapper)
        {
            _entityRepository = entityRepository;
            _mapper = mapper;
        }
        public async Task<EntityVm> Handle(GetEntityByIdQuery request, CancellationToken cancellationToken)
        {
          var entity = await _entityRepository.GetByIdAsync(request.EntityId);
          return _mapper.Map<EntityVm>(entity);
        }
    }
}