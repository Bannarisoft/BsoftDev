using MediatR;
using System.Data;
using Core.Application.Common.Interfaces.IEntity;
using AutoMapper;
using Core.Application.Entity.Queries.GetEntity;

namespace Core.Application.Entity.Queries.GetEntityLastCode
{
    public class GetEntityLastCodeQueryHandler : IRequestHandler<GetEntityLastCodeQuery,string>
    {
    private readonly IEntityQueryRepository _entityRepository;        
    private readonly IMapper _mapper;

    public GetEntityLastCodeQueryHandler(IEntityQueryRepository entityRepository,  IMapper mapper)
    {
         _entityRepository = entityRepository;
         _mapper =mapper;
    }

        public async Task<string> Handle(GetEntityLastCodeQuery request, CancellationToken cancellationToken)
        {
           var entityCode = await _entityRepository.GenerateEntityCodeAsync();
           return entityCode;
        }       

    }
}