using MediatR;
using System.Data;
using Core.Application.Common.Interfaces.IEntity;
using AutoMapper;
using Core.Application.Entity.Queries.GetEntity;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Entity.Queries.GetEntityLastCode
{
    public class GetEntityLastCodeQueryHandler : IRequestHandler<GetEntityLastCodeQuery,ApiResponseDTO<string>>
    {
    private readonly IEntityQueryRepository _entityRepository;        
    private readonly IMapper _mapper;

    public GetEntityLastCodeQueryHandler(IEntityQueryRepository entityRepository,  IMapper mapper)
    {
         _entityRepository = entityRepository;
         _mapper =mapper;
    }

        public async Task<ApiResponseDTO<string>> Handle(GetEntityLastCodeQuery request, CancellationToken cancellationToken)
        {
           var entityCode = await _entityRepository.GenerateEntityCodeAsync();
           return new ApiResponseDTO<string> { IsSuccess = true, Message = "Success", Data = entityCode };
        }       

    }
}