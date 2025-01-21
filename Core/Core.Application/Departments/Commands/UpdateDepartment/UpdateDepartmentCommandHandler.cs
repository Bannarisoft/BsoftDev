using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Departments.Queries.GetDepartments;
using Core.Application.Common.Interfaces.IDepartment;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;
using Core.Application.Common;


namespace Core.Application.Departments.Commands.UpdateDepartment
{

    public class UpdateDepartmentCommandHandler  : IRequestHandler<UpdateDepartmentCommand ,ApiResponseDTO<DepartmentDto>>
    {

        public readonly IDepartmentCommandRepository _IDepartmentCommandRepository;
       private readonly IMapper _Imapper;
        private readonly ILogger<UpdateDepartmentCommandHandler> _logger;
        private readonly IDepartmentQueryRepository _departmentQueryRepository;
        private readonly IMediator _mediator; 
         

        public UpdateDepartmentCommandHandler(IDepartmentCommandRepository iDepartmentcommandRepository,IDepartmentQueryRepository idepartmentQueryRepository, IMapper Imapper, ILogger<UpdateDepartmentCommandHandler> logger ,IMediator mediator  )
        {

            _IDepartmentCommandRepository = iDepartmentcommandRepository;
            _departmentQueryRepository = idepartmentQueryRepository;
            _Imapper = Imapper;
            _logger = logger;
             _mediator = mediator;
        }

    

       public async Task<ApiResponseDTO<DepartmentDto>> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
       {            
            _logger.LogInformation("Starting UpdateDepartmentCommandHandler for request: {@Request}", request);

            // Fetch the department by ID
            var department = await _departmentQueryRepository.GetByIdAsync(request.Id);
            if (department == null)
            {
                _logger.LogWarning("Department with ID {DepartmentId} not found.", request.Id);
                return new ApiResponseDTO<DepartmentDto>
                {
                    IsSuccess = false,
                    Message = "Department not found"
                };
            }

            _logger.LogInformation("Department with ID {DepartmentId} retrieved successfully.", request.Id);

            // Update department properties
            department.DeptName = request.DeptName;
            department.ShortName = request.ShortName;
            department.CompanyId = request.CompanyId;
            department.IsActive = request.IsActive;

            // Save updates to the repository
            var result = await _IDepartmentCommandRepository.UpdateAsync(request.Id, department);

            if (result == null)
            {
                _logger.LogWarning("Failed to update Department with ID {DepartmentId}.", request.Id);
                return new ApiResponseDTO<DepartmentDto>
                {
                    IsSuccess = false,
                    Message = "Failed to update department"
                };
            }

            _logger.LogInformation("Department with ID {DepartmentId} updated successfully.", request.Id);

            // Map the updated entity to DTO
            var dept = await _departmentQueryRepository.GetByIdAsync(request.Id);
            var departmentDto = _Imapper.Map<DepartmentDto>(dept);
           // var departmentDto = _Imapper.Map<DepartmentDto>(result);

            // Publish domain event for audit logs
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: department.Id.ToString(),
                actionName: department.DeptName,
                details: $"Department '{department.DeptName}' was updated. Department ID: {request.Id}",
                module: "Department"
            );

            await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation("AuditLogsDomainEvent published for Department ID {DepartmentId}.", department.Id);

            return new ApiResponseDTO<DepartmentDto>
            {
                IsSuccess = true,
                Message = "Department updated successfully"
               
            };
//              var department = await _departmentQueryRepository.GetByIdAsync(request.Id);

// 			 department.DeptName = request.DeptName; // Update properties based on the request
//             department.CompanyId= request.CompanyId;
//             department.IsActive= request.IsActive;

//             var result = await _IDepartmentCommandRepository.UpdateAsync(request.Id, department);
            
//             var departmentDto = _Imapper.Map<DepartmentDto>(result);

//  // Publish a domain event for audit logs
//     var domainEvent = new AuditLogsDomainEvent(
//         actionDetail: "Update",
//         actionCode: department.Id.ToString(),
//         actionName: department.DeptName,
//         details: $"Department '{department.DeptName}' was updated. Department ID: {request.Id}",
//         module: "Department"
//     );
//     await _mediator.Publish(domainEvent, cancellationToken);
          
//             return new ApiResponseDTO<DepartmentDto> { IsSuccess = true, Message = "Department updated successfully", Data = departmentDto };

  

       }


    }
}