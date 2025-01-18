using System.ComponentModel;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IEntity;
using Core.Domain.Events;
using MediatR;


namespace Core.Application.Entity.Queries.GetEntity
{
    public class GetEntityQueryHandler : IRequestHandler<GetEntityQuery, ApiResponseDTO<List<EntityDto>>>
    {
         private readonly IEntityQueryRepository _entityRepository;        
        private readonly IMapper _mapper;

        private readonly IMediator _mediator;
        public GetEntityQueryHandler(IEntityQueryRepository entityRepository,  IMapper mapper, IMediator mediator)
        {
           _entityRepository = entityRepository;
            _mapper =mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<List<EntityDto>>> Handle(GetEntityQuery request, CancellationToken cancellationToken)
        {
          
               
                var newentity = await _entityRepository.GetAllEntityAsync();
                var entitylist = _mapper.Map<List<EntityDto>>(newentity);
                
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetEntityQuery",
                    actionCode: "",        
                    actionName: "",
                    details: $"Entity details was fetched.",
                    module:"Entity"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
                return new ApiResponseDTO<List<EntityDto>>
                { 
                    IsSuccess = true, 
                    Message = "Success", 
                    Data = entitylist 
                 };
                
       

        }
    }
}