using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommandHandler  : IRequestHandler<UpdateDepartmentCommand ,int>
    {
        public readonly IDepartmentRepository _departmentRepository;

        public UpdateDepartmentCommandHandler(IDepartmentRepository departmentRepository)
        {
            _departmentRepository =departmentRepository;
        }

        public async Task<int>Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var UpdatedepartmentEntity = new Core.Domain.Entities.Department()
            {

                Id=request.Id,
                ShortName =request.ShortName,
                DeptName =request.DeptName,
                CompanyId =request.CompanyId,
                IsActive =request.IsActive
               

            };
            return await _departmentRepository.UpdateAsync(request.Id,UpdatedepartmentEntity);
        
        }
    }
}