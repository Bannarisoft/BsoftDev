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
using Core.Domain.Enums.Common;


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
            // public async Task<ApiResponseDTO<DepartmentDto>> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
            // {
            //     _logger.LogInformation("Starting UpdateDepartmentCommandHandler for request: {@Request}", request);

            //     // Check if the department exists
            //     var existing = await _departmentQueryRepository.GetByIdAsync(request.Id);
            //     if (existing == null)
            //     {
            //         _logger.LogWarning("Department with ID {DepartmentId} not found.", request.Id);
            //         return new ApiResponseDTO<DepartmentDto>
            //         {
            //             IsSuccess = false,
            //             Message = "Department not found."
            //         };
            //     }

            //     // Check for duplicate name before updating
            //     var duplicateCheck = await _IDepartmentCommandRepository.ExistsByNameupdateAsync(request.DeptName, request.Id);
            //     if (duplicateCheck)
            //     {
            //         return new ApiResponseDTO<DepartmentDto>
            //         {
            //             IsSuccess = false,
            //             Message = "Department Name already exists."
            //         };
            //     }

            //     // Map and update
            //     var departmentMap = _Imapper.Map<Department>(request);
            //     var result = await _IDepartmentCommandRepository.UpdateAsync(request.Id, departmentMap);

            //     if (result <= 0)
            //     {
            //         _logger.LogWarning("Failed to update Department with ID {DepartmentId}.", request.Id);
            //         return new ApiResponseDTO<DepartmentDto>
            //         {
            //             IsSuccess = false,
            //             Message = "Failed to update department."
            //         };
            //     }

            //     // Fetch updated department and map to DTO
            //     var updatedEntity = await _departmentQueryRepository.GetByIdAsync(request.Id);
            //     var departmentDto = _Imapper.Map<DepartmentDto>(updatedEntity);

            //     // Publish audit event
            //     var domainEvent = new AuditLogsDomainEvent(
            //         actionDetail: "Update",
            //         actionCode: departmentMap.Id.ToString(),
            //         actionName: departmentMap.DeptName,
            //         details: $"Department '{departmentMap.DeptName}' was updated. Department ID: {request.Id}",
            //         module: "Department"
            //     );
            //     await _mediator.Publish(domainEvent, cancellationToken);

            //     _logger.LogInformation("Department with ID {DepartmentId} updated successfully.", request.Id);

            //     return new ApiResponseDTO<DepartmentDto>
            //     {
            //         IsSuccess = true,
            //         Message = "Department updated successfully.",
            //         Data = departmentDto
            //     };
            // }

    

       public async Task<ApiResponseDTO<DepartmentDto>> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
       {            
            _logger.LogInformation("Starting UpdateDepartmentCommandHandler for request: {@Request}", request);
      

            _logger.LogInformation("Department with ID {DepartmentId} retrieved successfully.", request.Id);

            var departmentMap  = _Imapper.Map<Department>(request);
            // Save updates to the repository
            var result = await _IDepartmentCommandRepository.UpdateAsync(request.Id, departmentMap);
           // var resultCode = result ?? 0;
            if(result <= 0)
            {
                _logger.LogWarning("Failed to update Department with ID {DepartmentId}.", request.Id);
                return new ApiResponseDTO<DepartmentDto>
                {
                    IsSuccess = false,
                    Message = "Failed to update department"
                };
            }
            
              var duplicateCheck = await _IDepartmentCommandRepository.ExistsByNameupdateAsync(request.DeptName, request.Id);
                if (duplicateCheck)
                {
                      return new ApiResponseDTO<DepartmentDto>
                    {
                    IsSuccess = false,
                    Message = " Department Name  Already Exists "
                    };
                }

            _logger.LogInformation("Department with ID {DepartmentId} updated successfully.", request.Id);

            // Map the updated entity to DTO
            var dept = await _departmentQueryRepository.GetByIdAsync(request.Id);
            var departmentDto = _Imapper.Map<DepartmentDto>(dept);
           
            // Publish domain event for audit logs
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: departmentMap.Id.ToString(),
                actionName: departmentMap.DeptName,
                details: $"Department '{departmentMap.DeptName}' was updated. Department ID: {request.Id}",
                module: "Department"
            );

            await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation("AuditLogsDomainEvent published for Department ID {DepartmentId}.", departmentMap.Id);

            return new ApiResponseDTO<DepartmentDto>
            {
                IsSuccess = true,
                Message = "Department updated successfully"
               
            };


       }


    }
}