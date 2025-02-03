using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using Core.Application.Common.Interfaces.IDepartment;
using Core.Application.Departments.Queries.GetDepartments;
using Core.Domain.Events;
using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using Microsoft.Extensions.Logging;

namespace Core.Application.Departments.Queries.GetDepartmentAutoCompleteSearch
{

    public class GetDepartmentAutoCompleteSearchQueryHandler : IRequestHandler<GetDepartmentAutoCompleteSearchQuery, ApiResponseDTO<List<DepartmentAutocompleteDto>>>
    {
        private readonly IDepartmentQueryRepository _departmentRepository;
        private readonly IMapper _mapper;

          private readonly IMediator _mediator; 

          private readonly ILogger<GetDepartmentAutoCompleteSearchQueryHandler> _logger;
        public GetDepartmentAutoCompleteSearchQueryHandler(IDepartmentQueryRepository divisionRepository,IMapper mapper, IMediator mediator, ILogger<GetDepartmentAutoCompleteSearchQueryHandler> logger)
        {
             _mapper =mapper;

            _departmentRepository = divisionRepository;    
            _mediator = mediator;        
            _logger = logger;
        }


        public async Task<ApiResponseDTO<List<DepartmentAutocompleteDto>>> Handle(GetDepartmentAutoCompleteSearchQuery request, CancellationToken cancellationToken)
        { 

            _logger.LogInformation("Handling GetDepartmentAutoCompleteSearchQuery with search pattern: {SearchPattern}", request.SearchPattern);

             // Fetch departments matching the search pattern
                var result = await _departmentRepository.GetAllDepartmentAutoCompleteSearchAsync(request.SearchPattern);
                if (result == null || !result.Any())
                {
                    _logger.LogWarning("No departments found for search pattern: {SearchPattern}", request.SearchPattern);
                    return new ApiResponseDTO<List<DepartmentAutocompleteDto>>
                    {
                        IsSuccess = false,
                        Message = "No matching departments found",
                        Data = new List<DepartmentAutocompleteDto>()
                    };
                }

                _logger.LogInformation("Departments found for search pattern: {SearchPattern}. Mapping results to DTO.", request.SearchPattern);

                // Map the result to DTO
                var deptDto = _mapper.Map<List<DepartmentAutocompleteDto>>(result);

                // Publish domain event for audit logs
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAutoComplete",
                    actionCode: "",
                    actionName: request.SearchPattern,
                    details: $"Department '{request.SearchPattern}' was searched",
                    module: "Department"
                );
                await _mediator.Publish(domainEvent, cancellationToken);

                _logger.LogInformation("Domain event published for search pattern: {SearchPattern}", request.SearchPattern);

                return new ApiResponseDTO<List<DepartmentAutocompleteDto>>
                {
                    IsSuccess = true,
                    Message = "Success",
                    Data = deptDto
                };
//                   var result = await _departmentRepository.GetAllDepartmentAutoCompleteSearchAsync(request.SearchPattern);
//                   var department =_mapper.Map<ApiResponseDTO<List<DepartmentDto>>>(result);   

//  var deptDto = _mapper.Map<List<DepartmentDto>>(result);
//                 //Domain Event
//                 var domainEvent = new AuditLogsDomainEvent(
//                     actionDetail: "GetAutoComplete",
//                     actionCode:"",        
//                     actionName: request.SearchPattern,                
//                     details: $"Department '{request.SearchPattern}' was searched",
//                     module:"Department"
//                 );
//                 await _mediator.Publish(domainEvent, cancellationToken);
//                 return new ApiResponseDTO<List<DepartmentDto>> { IsSuccess = true, Message = "Success", Data = department.Data };  

        }
    }
         
}

