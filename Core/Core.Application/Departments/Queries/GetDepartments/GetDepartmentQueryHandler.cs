using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Data;
using Core.Domain.Events;
using Core.Application.Common.Interfaces.IDepartment;
using Core.Application.Common;


namespace Core.Application.Departments.Queries.GetDepartments
{
    public class GetDepartmentQueryHandler :IRequestHandler<GetDepartmentQuery,Result<List<DepartmentDto>>>
    {
        private readonly IDepartmentQueryRepository _departmentRepository;
        private readonly IMapper _mapper; 
         private readonly IMediator _mediator; 

     public GetDepartmentQueryHandler(IDepartmentQueryRepository divisionRepository,IMapper mapper , IMediator mediator)
        {
            _mapper =mapper;
            _departmentRepository = divisionRepository; 
              _mediator = mediator;                
        }

        public async Task<Result<List<DepartmentDto>>> Handle(GetDepartmentQuery request ,CancellationToken cancellationToken )
        {

            var department = await _departmentRepository.GetAllDepartmentAsync();
             var departmentList = _mapper.Map<List<DepartmentDto>>(department);
              
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "",
                    details: $"Department details was fetched.",
                    module:"Department"
                );

                  await _mediator.Publish(domainEvent, cancellationToken);
                return Result<List<DepartmentDto>>.Success(departmentList);


          
           
        }

      
    }
}