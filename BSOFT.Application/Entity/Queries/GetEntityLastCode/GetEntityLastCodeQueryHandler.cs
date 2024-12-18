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


namespace BSOFT.Application.Entity.Queries.GetEntityLastCode
{
    public class GetEntityLastCodeQueryHandler : IRequestHandler<GetEntityLastCodeQuery,string>
    {
     private readonly IEntityRepository _entityRepository;
     private readonly IMapper _mapper;

       public GetEntityLastCodeQueryHandler(IEntityRepository entityRepository, IMapper mapper)
        {
            _entityRepository = entityRepository;
            _mapper = mapper;
        }

          // Last Entity Code Check 

        public async Task<string> Handle(GetEntityLastCodeQuery request, CancellationToken cancellationToken)
       {
            var lastentitycodecheck = await _entityRepository.GenerateEntityCodeAsync();
            return _mapper.Map<string>(lastentitycodecheck);
       
       }


    }
}