using AutoMapper;
using Core.Application.Common.Interfaces.IEntity;
using MediatR;


namespace Core.Application.Entity.Queries.GetEntity
{
    public class GetEntityQueryHandler : IRequestHandler<GetEntityQuery, List<EntityDto>>
    {
         private readonly IEntityQueryRepository _entityRepository;        
        private readonly IMapper _mapper;
        public GetEntityQueryHandler(IEntityQueryRepository entityRepository,  IMapper mapper)
        {
           _entityRepository = entityRepository;
            _mapper =mapper;
        }
        public async Task<List<EntityDto>> Handle(GetEntityQuery request, CancellationToken cancellationToken)
        {
          /*   const string query = @"
            SELECT *
            FROM AppData.Entity";
        var Entities = await _dbConnection.QueryAsync<EntityDto>(query);
        return Entities.AsList(); */
         var users = await _entityRepository.GetAllEntityAsync();
            var userList = _mapper.Map<List<EntityDto>>(users);
            return userList;

        }
    }
}