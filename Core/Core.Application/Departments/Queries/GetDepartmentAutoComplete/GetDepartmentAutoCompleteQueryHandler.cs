using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Core.Domain.Entities;
using Core.Application.Departments.Queries.GetDepartments;
using Core.Application.Common.Interfaces.IDepartment;
using Core.Application.Common.HttpResponse;


namespace Core.Application.Departments.Queries.GetDepartmentAutoComplete
{
    public class GetDepartmentAutoCompleteQueryHandler : IRequestHandler<GetDepartmentAutoCompleteQuery,ApiResponseDTO<List<DepartmentDto>>>
    {
        
        private readonly IDepartmentQueryRepository _departmentRepository;
        private readonly IMapper _mapper;

      public GetDepartmentAutoCompleteQueryHandler(IDepartmentQueryRepository divisionRepository,IMapper mapper)
      {
            _mapper =mapper;
            _departmentRepository = divisionRepository;   
      }

      public async Task<ApiResponseDTO<List<DepartmentDto>>> Handle(GetDepartmentAutoCompleteQuery request, CancellationToken cancellationToken)
    {
          

        var result = await _departmentRepository.GetAllDepartmentAutoCompleteSearchAsync(request.SearchPattern);
            var department = _mapper.Map<List<DepartmentDto>>(result);
        return new ApiResponseDTO<List<DepartmentDto>> { IsSuccess = true, Message = "Success", Data = department };     

    }
     

    }
}