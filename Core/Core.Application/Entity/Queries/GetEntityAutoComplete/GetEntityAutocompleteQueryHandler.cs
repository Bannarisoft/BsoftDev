using System.Data;
using MediatR;
using Core.Application.Entity.Queries.GetEntity;
using Core.Application.Common.Interfaces.IEntity;
using AutoMapper;
using Core.Application.Common;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;
using Microsoft.Extensions.Logging;

namespace Core.Application.Entity.Queries.GetEntityAutoComplete
{
    public class GetEntityAutocompleteQueryHandler : IRequestHandler<GetEntityAutocompleteQuery, ApiResponseDTO<List<GetEntityDTO>>>
    {
        private readonly IEntityQueryRepository _entityRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        private readonly ILogger<GetEntityAutocompleteQueryHandler> _logger;


    public GetEntityAutocompleteQueryHandler(IEntityQueryRepository entityRepository,  IMapper mapper,IMediator mediator,ILogger<GetEntityAutocompleteQueryHandler> logger)
    {
         _entityRepository = entityRepository;
         _mapper =mapper;
         _mediator = mediator;
         _logger = logger?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApiResponseDTO<List<GetEntityDTO>>> Handle(GetEntityAutocompleteQuery request, CancellationToken cancellationToken)
    {

                 _logger.LogInformation($"Search pattern started: {request.SearchPattern}");
                var entities = await _entityRepository.GetByEntityNameAsync(request.SearchPattern);

                if (entities is null || !entities.Any() || entities.Count == 0)
                {
                 _logger.LogWarning($"No Entity Record {request.SearchPattern} not found in DB.");
                     return new ApiResponseDTO<List<GetEntityDTO>>
                     {
                         IsSuccess = false,
                         Message = "No entity found"
                     };
                }
                var entityDto = _mapper.Map<List<GetEntityDTO>>(entities);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetEntityAutocompleteQuery",
                    actionCode:"Get Entity Autocomplete",        
                    actionName: request.SearchPattern,                
                    details: $"Entity '{request.SearchPattern}' was searched",
                    module:"Entity"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
                 _logger.LogInformation($"Entity {entities.Count} Listed successfully.");
                return new ApiResponseDTO<List<GetEntityDTO>>
                {
                    IsSuccess = true,
                    Message = "Success",
                    Data = entityDto
                };
           
            }            

    }
    }
    
    
