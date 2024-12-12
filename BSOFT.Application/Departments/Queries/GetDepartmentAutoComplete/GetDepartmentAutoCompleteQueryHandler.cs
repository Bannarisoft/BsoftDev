using AutoMapper;
using BSOFT.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BSOFT.Application.Departments.Queries.GetDepartmentAutoComplete
{
    public class GetDepartmentAutoCompleteQueryHandler : IRequestHandler<GetDepartmentAutoCompleteQuery,List<DepartmentAutoCompleteVm>>
    {
        
      private readonly IDepartmentRepository _departmentRepository;
      private readonly IMapper _mapper;

      public GetDepartmentAutoCompleteQueryHandler(IDepartmentRepository departmentRepository,IMapper mapper)
      {
        _departmentRepository =departmentRepository;
        _mapper =mapper;
      }
      public async Task<List<DepartmentAutoCompleteVm>>Handle(GetDepartmentAutoCompleteQuery request, CancellationToken cancellationToken)
      {
        var deparment =await _departmentRepository.GetAllDepartmentAsync();
    var departmentList=_mapper.Map<List<DepartmentAutoCompleteVm>>(deparment);
    return departmentList;
      }

    }
}