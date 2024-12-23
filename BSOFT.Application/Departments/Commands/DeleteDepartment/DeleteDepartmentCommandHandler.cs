using MediatR;
using BSOFT.Application.Common.Interfaces;
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
                Id = request.Id,
                IsActive = request.IsActive                

            };
            return await _departmentRepository.DeleteAsync(request.Id,Updatedepartment);     
      }

    }
}