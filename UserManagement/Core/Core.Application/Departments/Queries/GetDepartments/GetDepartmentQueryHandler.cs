using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Data;
using Core.Domain.Events;
using Core.Application.Common.Interfaces.IDepartment;
using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using Microsoft.Extensions.Logging;


namespace Core.Application.Departments.Queries.GetDepartments
{

    public class GetDepartmentQueryHandler :IRequestHandler<GetDepartmentQuery,ApiResponseDTO<List<GetDepartmentDto>>>
    {
        private readonly IDepartmentQueryRepository _departmentRepository;
        private readonly IMapper _mapper; 
         private readonly IMediator _mediator; 

         private readonly ILogger<GetDepartmentQueryHandler> _logger;


     public GetDepartmentQueryHandler(IDepartmentQueryRepository divisionRepository,IMapper mapper , IMediator mediator, ILogger<GetDepartmentQueryHandler> logger)
        {
            _mapper =mapper;
            _departmentRepository = divisionRepository; 
            _mediator = mediator;     
            _logger = logger;

        }


        public async Task<ApiResponseDTO<List<GetDepartmentDto>>> Handle(GetDepartmentQuery request ,CancellationToken cancellationToken )
        {
             _logger.LogInformation("Fetching Department Request started: {request}", request);
           
            var (department,totalCount) = await _departmentRepository.GetAllDepartmentAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            
             if (department is null || !department.Any())
            {
               _logger.LogWarning("No department records found in the database. Total count: {Count}", department?.Count ?? 0);

                  return new ApiResponseDTO<List<GetDepartmentDto>> { IsSuccess = false, Message = "No Record Found" };
            }

             var departmentList = _mapper.Map<List<GetDepartmentDto>>(department);
             var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "",
                    details: $"Department details was fetched.",
                    module:"Department"
                );

                  await _mediator.Publish(domainEvent, cancellationToken);
              
            _logger.LogInformation("Department {department} Listed successfully.", departmentList.Count);
            return new ApiResponseDTO<List<GetDepartmentDto>> { IsSuccess = true,
            Message = "Success",
            Data = departmentList ,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize};  
           
        }

      
    }
}