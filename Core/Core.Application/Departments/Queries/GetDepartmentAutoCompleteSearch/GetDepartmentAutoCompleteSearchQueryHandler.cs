using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.Departments.Queries.GetDepartmentAutoComplete;


namespace Core.Application.Departments.Queries.GetDepartmentAutoCompleteSearch
{
    public class GetDepartmentAutoCompleteSearchQueryHandler : IRequestHandler<GetDepartmentAutoCompleteSearchQuery, List<DepartmentAutoCompleteVm>>
    {
       private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public GetDepartmentAutoCompleteSearchQueryHandler(IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        public async Task<List<DepartmentAutoCompleteVm>> Handle(GetDepartmentAutoCompleteSearchQuery request, CancellationToken cancellationToken)
        {



            
            var division = await _departmentRepository.GetAllDepartmentAutoCompleteSearchAsync(request.SearchDept);
            // Map to the application-specific DTO
            return division.Select(d => new DepartmentAutoCompleteVm
            {
                Id = d.Id,
                DeptName = d.DeptName
            }).ToList();
            
        }
        }
         
    }

