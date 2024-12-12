using MediatR;
using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace BSOFT.Application.Departments.Commands.DeleteDepartment
{
    public class DeleteDepartmentCommandHandler :IRequestHandler<DeleteDepartmentCommand ,int>
    {
      private readonly IDepartmentRepository _departmentRepository;  
      
      public DeleteDepartmentCommandHandler (IDepartmentRepository departmentRepository)
      {
        _departmentRepository =departmentRepository;
      }

      public async Task<int>Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
      {
         var Updatedepartment = new Department()
            {
                DeptId = request.DeptId,
                IsActive = request.IsActive ,
                ModifiedBy =request.ModifiedBy,
                ModifiedAt =request.ModifiedAt ,
                ModifiedByName=request.ModifiedByName,
                ModifiedIP=request.ModifiedIP

            };
            return await _departmentRepository.DeleteAsync(request.DeptId,Updatedepartment);

        //return await _departmentRepository.DeleteAsync(request.DeptId);
      }

    }
}