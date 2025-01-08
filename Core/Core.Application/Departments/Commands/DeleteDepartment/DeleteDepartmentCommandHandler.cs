using MediatR;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using  Core.Application.Departments.Queries.GetDepartments;
using Core.Application.Common.Interfaces.IDepartment;

namespace Core.Application.Departments.Commands.DeleteDepartment
{
    public class DeleteDepartmentCommandHandler :IRequestHandler<DeleteDepartmentCommand ,int>
    {
      private readonly IDepartmentCommandRepository _IdepartmentRepository;  
       private readonly IMapper _mapper;
      
      public DeleteDepartmentCommandHandler (IDepartmentCommandRepository departmentRepository , IMapper mapper)
      {
         _IdepartmentRepository = departmentRepository;
            _mapper = mapper;
      }

      public async Task<int>Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
      {       
          var updatedDepartment = _mapper.Map<Department>(request.departmentStatusDto);
            return await _IdepartmentRepository.DeleteAsync(request.Id, updatedDepartment);              
      }

    }
}