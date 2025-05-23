using MediatR;
using Core.Domain.Entities;
using AutoMapper;
using Core.Application.Common.Interfaces.IDepartment;
using Core.Application.Common.HttpResponse;
using Core.Domain.Events;
using Microsoft.Extensions.Logging;
using Contracts.Interfaces.External.IMaintenance;

namespace Core.Application.Departments.Commands.DeleteDepartment
{

    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, ApiResponseDTO<int>>
    {

        private readonly IDepartmentCommandRepository _IdepartmentCommandRepository;
        private readonly IMapper _Imapper;
        private readonly IDepartmentQueryRepository _IdepartmentQueryRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<DeleteDepartmentCommandHandler> _logger;
        private readonly IDepartmentValidationGrpcClient _departmentValidationGrpcClient;

        public DeleteDepartmentCommandHandler(IDepartmentCommandRepository departmentRepository, IDepartmentQueryRepository departmentQueryRepository, IMediator mediator, IMapper mapper, ILogger<DeleteDepartmentCommandHandler> logger, IDepartmentValidationGrpcClient departmentValidationGrpcClient)
        {

            _IdepartmentCommandRepository = departmentRepository;
            _Imapper = mapper;
            _IdepartmentQueryRepository = departmentQueryRepository;
            _mediator = mediator;
            _logger = logger;
            _departmentValidationGrpcClient = departmentValidationGrpcClient;
        }


        public async Task<ApiResponseDTO<int>> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("DeleteDepartmentCommandHandler started for Department ID: {DepartmentId}", request.Id);
            // ✅ Step 1: Call MaintenanceService via gRPC to check usage
            bool isUsed = await _departmentValidationGrpcClient.CheckIfDepartmentIsUsedAsync(request.Id);

            if (isUsed)
            {
                _logger.LogWarning("Cannot delete Department ID {DepartmentId} - it is in use by CostCenters.", request.Id);
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Cannot delete department. It is still in use in Maintenance system.",
                    Data = 0
                };
            }
            
            // Map request to entity and delete
            var updatedDepartment = _Imapper.Map<Department>(request);
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

        }

    }
}