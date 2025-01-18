using System.Data;
using MediatR;
using Core.Application.Entity.Queries.GetEntity;
using Core.Application.Common.Interfaces.IEntity;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common;
using Core.Domain.Events;

namespace Core.Application.Entity.Queries.GetEntityAutoComplete
{
    public class GetEntityAutocompleteQueryHandler : IRequestHandler<GetEntityAutocompleteQuery, Result<List<EntityDto>>>
    {
        private readonly IEntityQueryRepository _entityRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

    public GetEntityAutocompleteQueryHandler(IEntityQueryRepository entityRepository,  IMapper mapper,IMediator mediator)
    {
         _entityRepository = entityRepository;
         _mapper =mapper;
         _mediator = mediator;
    }

    public async Task<Result<List<EntityDto>>> Handle(GetEntityAutocompleteQuery request, CancellationToken cancellationToken)
    {

              
                var entities = await _entityRepository.GetByEntityNameAsync(request.SearchPattern);
                if (entities == null || !entities.Any())
                {
                return Result<List<EntityDto>>.Failure("Entity not found.");
                }
                var entityDto = _mapper.Map<List<EntityDto>>(entities);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetEntityAutocompleteQuery",
                    actionCode:"",        
                    actionName: request.SearchPattern,                
                    details: $"Entity '{request.SearchPattern}' was searched",
                    module:"Entity"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
                return Result<List<EntityDto>>.Success(entityDto);
           
            }        








    //    try
    // {
    //     // Check if searchPattern is provided
    //     if (string.IsNullOrWhiteSpace(request.SearchPattern))
    //     {
    //         // Optionally, you can throw a custom exception or return an empty list
    //         throw new CustomException(
    //             "Search pattern cannot be empty.",
    //             new[] { "Please provide a valid search pattern." },
    //             CustomException.HttpStatus.BadRequest
    //         );
    //     }

    //     // Fetch the entities based on the search pattern
    //     var result = await _entityRepository.GetByEntityNameAsync(request.SearchPattern);

    //     // Check if no entities were found
    //     if (result == null || !result.Any())
    //     {
    //         // You can throw a CustomException here if needed or just return an empty list
    //         throw new CustomException(
    //             "No entities found matching the search pattern.",
    //             new[] { $"No entities found for the search pattern: {request.SearchPattern}" },
    //             CustomException.HttpStatus.NotFound
    //         );
    //     }

    //     // Map the entity results to DTOs (Data Transfer Objects)
    //     var mappedResult = _mapper.Map<List<EntityDto>>(result);

    //     // Return the mapped result
    //     return mappedResult;
    // }
    // catch (Exception ex)
    // {
    //     // Log the error (if needed) and rethrow a custom exception
    //     // Optionally, log the exception using a logger here if needed
    //     throw new CustomException(
    //         "An error occurred while processing the request.",
    //         new[] { ex.Message },
    //         CustomException.HttpStatus.InternalServerError
    //     );
    // }           


    }
    }
    
    
