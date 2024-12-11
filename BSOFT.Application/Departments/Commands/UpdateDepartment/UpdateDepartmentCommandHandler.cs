using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Application.Departments.Commands.UpdateDepartment
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
            var UpdatedepartmentEntity = new BSOFT.Domain.Entities.Department()
            {

                DeptId=request.DeptId,
                ShortName =request.ShortName,
                DeptName =request.DeptName,
                CoId =request.CoId,
                IsActive =request.IsActive,

                ModifiedBy     =request.ModifiedBy,
                ModifiedAt  =request.ModifiedAt,
                ModifiedByName=request.ModifiedByName,
                ModifiedIP=request.ModifiedIP

            };
            return await _departmentRepository.UpdateAsync(request.DeptId,UpdatedepartmentEntity);
        
        }
    }
}