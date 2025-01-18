using System.ComponentModel;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces.IEntity;
using Core.Domain.Events;
using MediatR;


namespace Core.Application.Entity.Queries.GetEntity
{
    public class GetEntityQueryHandler : IRequestHandler<GetEntityQuery, Result<List<EntityDto>>>
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
        public async Task<Result<List<EntityDto>>> Handle(GetEntityQuery request, CancellationToken cancellationToken)
        {
          
                try
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
                return Result<List<EntityDto>>.Success(entitylist);
                }
                catch (Exception ex)    
                {
                     // Throw a generic CustomException for unexpected errors
                    throw new CustomException(
                    "An unexpected error occurred while Fetching the Entity.",
                    new[] { ex.Message },
                    CustomException.HttpStatus.InternalServerError
                    );
                }
       

        }
    }
}