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

namespace Core.Application.Departments.Commands.DeleteDepartment
{

    public class DeleteDepartmentCommandHandler :IRequestHandler<DeleteDepartmentCommand ,ApiResponseDTO<int>>
    {

      private readonly IDepartmentCommandRepository _IdepartmentCommandRepository;  
       private readonly IMapper _Imapper;          
        private readonly IDepartmentQueryRepository _IdepartmentQueryRepository;
   
        private readonly IMediator _mediator; 
      

      public DeleteDepartmentCommandHandler (IDepartmentCommandRepository departmentRepository ,IDepartmentQueryRepository departmentQueryRepository,IMediator mediator, IMapper mapper)
      {

         _IdepartmentCommandRepository = departmentRepository;
         _Imapper = mapper;                       
          _IdepartmentQueryRepository = departmentQueryRepository;
          _mediator = mediator;
      }


      public async Task<ApiResponseDTO<int>>Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
      {       
                   
          var updatedDepartment = _Imapper.Map<Department>(request.departmentStatusDto);
          
          var result = await _IdepartmentCommandRepository.DeleteAsync(request.Id, updatedDepartment);     

 //Domain Event
        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "Delete",
            actionCode: updatedDepartment.Id.ToString(),
            actionName:"",
            details:$"Department Id: {request.Id} was Changed to Status Inactive.",
            module:"Department"
        );
        await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<int> { IsSuccess = true, Message = "Department deleted successfully", Data = result };
      }

    }
}