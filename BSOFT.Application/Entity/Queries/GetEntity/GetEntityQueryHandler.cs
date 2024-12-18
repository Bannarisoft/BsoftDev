using AutoMapper;
using BSOFT.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Entity.Queries.GetEntity
{
    public class GetEntityQueryHandler : IRequestHandler<GetEntityQuery,List<EntityVm>>
    {
        private readonly IEntityRepository _entityRepository;
        private readonly IMapper _mapper;

        public GetEntityQueryHandler(IEntityRepository entityRepository , IMapper mapper)
        {
            _entityRepository = entityRepository;
            _mapper = mapper;
        }
        public async Task<List<EntityVm>> Handle(GetEntityQuery request, CancellationToken cancellationToken)
        {
            var entity = await _entityRepository.GetAllEntityAsync();
            var entityList = _mapper.Map<List<EntityVm>>(entity);
            return entityList;
        }

        
   
      
    }
}