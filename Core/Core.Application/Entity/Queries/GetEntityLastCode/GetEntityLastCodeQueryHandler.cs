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
       // Last Entity Code Check 
       public async Task<string> Handle(GetEntityLastCodeQuery request, CancellationToken cancellationToken)
       {
         /*  var query = "SELECT TOP 1 EntityCode FROM AppData.Entity ORDER BY Id DESC";
          var lastCode = await _dbConnection.QueryFirstOrDefaultAsync<string>(query);

          if (string.IsNullOrEmpty(lastCode))
          {
            lastCode = "ENT-00000";
          }

          var nextCodeNumber = int.Parse(lastCode[(lastCode.IndexOf('-') + 1)..]) + 1;

          return $"ENT-{nextCodeNumber:D5}"; */
        var users = await _entityRepository.GetAllEntityAsync();
            var userList = _mapper.Map<List<EntityDto>>(users);
            return userList.ToString();
       
       }


    }
}