using Core.Application.Departments.Queries.GetDepartments;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Core.Domain.Events;
using Core.Application.Common.Interfaces.IDepartment;
using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using Microsoft.Extensions.Logging;

namespace Core.Application.Departments.Queries.GetDepartmentById
{

    public class GetDepartmentByIdQueryHandler :IRequestHandler<GetDepartmentByIdQuery,ApiResponseDTO<GetDepartmentDto>>
    {
          private readonly IDepartmentQueryRepository _departmentRepository;        
        private readonly IMapper _mapper;
         private readonly IMediator _mediator;
           private readonly ILogger<GetDepartmentByIdQueryHandler> _logger;



        public GetDepartmentByIdQueryHandler(IDepartmentQueryRepository departmentRepository,IMapper mapper , IMediator mediator, ILogger<GetDepartmentByIdQueryHandler> logger)
         {
            _departmentRepository = departmentRepository;
            _mapper =mapper;
            _mediator = mediator;
            _logger = logger;
        } 

      public async Task<ApiResponseDTO<GetDepartmentDto>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            
                _logger.LogInformation("Fetching Department Request started: {Request}", request);

                    // Fetch department by ID
                    var department = await _departmentRepository.GetByIdAsync(request.DepartmentId);
                    
                    if (department == null)
                    {
                        _logger.LogWarning("Department with ID {DepartmentId} not found.", request.DepartmentId);

                        return new ApiResponseDTO<GetDepartmentDto>
                        {
                            IsSuccess = false,
                            Message = "Department not found.",
                            Data = null
                        };
                    }
            

              var deptDto = _mapper.Map<GetDepartmentDto>(department);
 //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: deptDto.Id.ToString(),        
                    actionName: deptDto.DeptName,                
                    details: $"Department '{deptDto.DeptName}' was created. DepartmentCode: {deptDto.Id}",
                    module:"Department"
                );

                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<GetDepartmentDto> { IsSuccess = true, Message = "Success", Data = deptDto };

               

           
        }
 


    }
}