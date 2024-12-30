using MediatR;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Departments.Commands.DeleteDepartment
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