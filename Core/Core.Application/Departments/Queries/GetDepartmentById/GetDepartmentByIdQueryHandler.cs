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

namespace Core.Application.Departments.Queries.GetDepartmentById
{

    public class GetDepartmentByIdQueryHandler :IRequestHandler<GetDepartmentByIdQuery,ApiResponseDTO<DepartmentDto>>
    {
          private readonly IDepartmentQueryRepository _departmentRepository;        
        private readonly IMapper _mapper;
         private readonly IMediator _mediator;


        public GetDepartmentByIdQueryHandler(IDepartmentQueryRepository departmentRepository,IMapper mapper , IMediator mediator)
         {
            _departmentRepository = departmentRepository;
            _mapper =mapper;
            _mediator = mediator;
        } 

      public async Task<ApiResponseDTO<DepartmentDto>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {

                 var department = await _departmentRepository.GetByIdAsync(request.DepartmentId);
              var deptDto = _mapper.Map<DepartmentDto>(department);
 //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: deptDto.Id.ToString(),        
                    actionName: deptDto.DeptName,                
                    details: $"Department '{deptDto.DeptName}' was created. DepartmentCode: {deptDto.Id}",
                    module:"Department"
                );

                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<DepartmentDto> { IsSuccess = true, Message = "Success", Data = deptDto };

               

           
        }
 


    }
}