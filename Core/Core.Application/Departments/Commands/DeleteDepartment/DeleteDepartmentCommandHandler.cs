using MediatR;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using  Core.Application.Departments.Queries.GetDepartments;
using Core.Application.Common.Interfaces.IDepartment;
using Core.Application.Departments.Commands.DeleteDepartment;
using Core.Application.Common.HttpResponse;
using Core.Domain.Events;
using Core.Application.Common;
using Microsoft.Extensions.Logging;

namespace Core.Application.Departments.Commands.DeleteDepartment
{

    public class DeleteDepartmentCommandHandler :IRequestHandler<DeleteDepartmentCommand ,ApiResponseDTO<int>>
    {

      private readonly IDepartmentCommandRepository _IdepartmentCommandRepository;  
       private readonly IMapper _Imapper;          
        private readonly IDepartmentQueryRepository _IdepartmentQueryRepository;
   
        private readonly IMediator _mediator; 
             private readonly ILogger<DeleteDepartmentCommandHandler> _logger;
      

      public DeleteDepartmentCommandHandler (IDepartmentCommandRepository departmentRepository ,IDepartmentQueryRepository departmentQueryRepository,IMediator mediator, IMapper mapper,ILogger<DeleteDepartmentCommandHandler> logger)
      {

         _IdepartmentCommandRepository = departmentRepository;
         _Imapper = mapper;                       
          _IdepartmentQueryRepository = departmentQueryRepository;
          _mediator = mediator;
          _logger = logger;
      }


      public async Task<ApiResponseDTO<int>>Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
      {       

                  _logger.LogInformation("DeleteDepartmentCommandHandler started for Department ID: {DepartmentId}", request.Id);

            // Check if department exists
            var department = await _IdepartmentQueryRepository.GetByIdAsync(request.Id);
            if (department == null)
            {
                _logger.LogWarning("Department with ID {DepartmentId} not found.", request.Id);
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Department not found",
                    Data = 0
                };
            }

            _logger.LogInformation("Department with ID {DepartmentId} found. Proceeding with deletion.", request.Id);

            // Map request to entity and delete
            var updatedDepartment = _Imapper.Map<Department>(request.departmentStatusDto);
            var result = await _IdepartmentCommandRepository.DeleteAsync(request.Id, updatedDepartment);

            if (result <= 0)
            {
                _logger.LogWarning("Failed to delete Department with ID {DepartmentId}.", request.Id);
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Failed to delete department",
                    Data = result
                };
            }

            _logger.LogInformation("Department with ID {DepartmentId} deleted successfully.", request.Id);

            // Publish domain event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: updatedDepartment.Id.ToString(),
                actionName: "",
                details: $"Department ID: {request.Id} was changed to status inactive.",
                module: "Department"
            );

            await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation("AuditLogsDomainEvent published for Department ID {DepartmentId}.", request.Id);

            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "Department deleted successfully",
                Data = result
            };  
//           var updatedDepartment = _Imapper.Map<Department>(request.departmentStatusDto);
          
//           var result = await _IdepartmentCommandRepository.DeleteAsync(request.Id, updatedDepartment);     

//  //Domain Event
//         var domainEvent = new AuditLogsDomainEvent(
//             actionDetail: "Delete",
//             actionCode: updatedDepartment.Id.ToString(),
//             actionName:"",
//             details:$"Department Id: {request.Id} was Changed to Status Inactive.",
//             module:"Department"
//         );
//         await _mediator.Publish(domainEvent, cancellationToken);

//             return new ApiResponseDTO<int> { IsSuccess = true, Message = "Department deleted successfully", Data = result };
      }

    }
}