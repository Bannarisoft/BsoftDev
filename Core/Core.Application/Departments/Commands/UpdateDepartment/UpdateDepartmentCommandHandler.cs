using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Core.Application.Departments.Queries.GetDepartments;
using Core.Application.Common.Interfaces.IDepartment;

namespace Core.Application.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommandHandler  : IRequestHandler<UpdateDepartmentCommand ,DepartmentDto>
    {
        public readonly IDepartmentCommandRepository _IDepartmentRepository;
       private readonly IMapper _Imapper;
        private readonly ILogger<UpdateDepartmentCommandHandler> _logger;

        public UpdateDepartmentCommandHandler(IDepartmentCommandRepository iDepartmentRepository, IMapper Imapper, ILogger<UpdateDepartmentCommandHandler> logger)
        {
            _IDepartmentRepository = iDepartmentRepository;
            _Imapper = Imapper;
            _logger = logger;
        }

    
       public async Task<DepartmentDto> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
       {

     
            var department = _Imapper.Map<Department>(request);
            await _IDepartmentRepository.UpdateAsync(request.Id, department);
            var departmentDto = _Imapper.Map<DepartmentDto>(department);
          
            return departmentDto;

       }


    }
}