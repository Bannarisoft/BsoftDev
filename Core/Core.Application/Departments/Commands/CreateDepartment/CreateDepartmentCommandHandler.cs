using Core.Application.Departments.Queries.GetDepartments;
using Core.Application.Departments.Commands.CreateDepartment;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IDepartment;
using Core.Application.Common;
using Core.Domain.Events;

namespace Core.Application.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandHandler :IRequestHandler<CreateDepartmentCommand, Result<DepartmentDto>>
    {
        private readonly IDepartmentCommandRepository _departmentRepository;
        private readonly IMapper _mapper;
          private readonly IMediator _mediator; 
           
    
         public CreateDepartmentCommandHandler(IDepartmentCommandRepository departmentRepository,IMapper mapper, IMediator mediator)
        {
             _departmentRepository=departmentRepository;
            _mapper=mapper;
            _mediator=mediator;

        }

     

       public async Task<Result<DepartmentDto>>Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {

       
           var departmentEntity = _mapper.Map<Department>(request);

             var result = await _departmentRepository.CreateAsync(departmentEntity);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: result.Id.ToString(),
                actionName: result.DeptName,
                details: $"Country '{result.DeptName}' was created. CountryCode: {result.Id}",
                module:"Country"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            var DeptDto = _mapper.Map<DepartmentDto>(result);
            return Result<DepartmentDto>.Success(DeptDto);
        }
    }  
}