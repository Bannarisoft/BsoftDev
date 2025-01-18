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
using Core.Application.Common;


namespace Core.Application.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommandHandler  : IRequestHandler<UpdateDepartmentCommand ,Result<int>>
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

    
    //   public async Task<Result<int>>Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)

       public async Task<Result<int>> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
       {

             // Fetch the existing department by ID
    var department = await _departmentQueryRepository.GetByIdAsync(request.Id);

    if (department == null || department.IsActive != 1)
    {
        return Result<int>.Failure("Invalid DepartmentId. The specified Department does not exist or is inactive.");
    }

    // Map the request to the department entity
    department.DeptName = request.DeptName; // Update properties based on the request
    department.CompanyId= request.CompanyId;
    department.IsActive= request.IsActive;

    // Update the department in the repository
    var result = await _IDepartmentCommandRepository.UpdateAsync(request.Id, department);

    if (result == 0) // Assuming '0' indicates a failure
    {
        return Result<int>.Failure("Failed to update the department.");
    }

    // Publish a domain event for audit logs
    var domainEvent = new AuditLogsDomainEvent(
        actionDetail: "Update",
        actionCode: department.Id.ToString(),
        actionName: department.DeptName,
        details: $"Department '{department.DeptName}' was updated. Department ID: {request.Id}",
        module: "Department"
    );
    await _mediator.Publish(domainEvent, cancellationToken);

    // Return the result
    return Result<int>.Success(result);

       }


    }
}