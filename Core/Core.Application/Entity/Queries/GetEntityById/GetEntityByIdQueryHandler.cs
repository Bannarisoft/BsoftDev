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
       /*  var query = "SELECT * FROM AppData.Entity WHERE Id = @Id";
        var entityresult = await _dbConnection.QuerySingleOrDefaultAsync<EntityDto>(query, new { Id = request.EntityId });
        // Return null if the country is not found
        if (entityresult == null)
        {
            return null;
        }
        // Map the entity to a DTO
        return new EntityDto
        {
            Id = entityresult.Id,
            EntityCode = entityresult.EntityCode,
            EntityName = entityresult.EntityName,
            EntityDescription = entityresult.EntityDescription,
            Address= entityresult.Address,
            Phone= entityresult.Phone,
            Email= entityresult.Email,
            IsActive = entityresult.IsActive
        }; */
            var result = await _entityRepository.GetByIdAsync(request.EntityId);
          return _mapper.Map<EntityDto>(result);
    }
    }
}