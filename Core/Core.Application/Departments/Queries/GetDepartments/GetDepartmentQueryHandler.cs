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

    public class GetDepartmentQueryHandler :IRequestHandler<GetDepartmentQuery,ApiResponseDTO<List<DepartmentDto>>>
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


        public async Task<ApiResponseDTO<List<DepartmentDto>>> Handle(GetDepartmentQuery request ,CancellationToken cancellationToken )
        {
             _logger.LogInformation("Fetching Department Request started: {request}", request);
           
           
             var department = await _departmentRepository.GetAllDepartmentAsync();
            
             if (department == null || !department.Any())
            {
               _logger.LogWarning("No department records found in the database. Total count: {Count}", department?.Count ?? 0);

                  return new ApiResponseDTO<List<DepartmentDto>> { IsSuccess = false, Message = "No Record Found" };
            }

             var departmentList = _mapper.Map<List<DepartmentDto>>(department);
             var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "",
                    details: $"Department details was fetched.",
                    module:"Department"
                );

                  await _mediator.Publish(domainEvent, cancellationToken);
              
            _logger.LogInformation("Department {department} Listed successfully.", departmentList.Count);
            return new ApiResponseDTO<List<DepartmentDto>> { IsSuccess = true, Message = "Success", Data = departmentList };  
           
        }

      
    }
}