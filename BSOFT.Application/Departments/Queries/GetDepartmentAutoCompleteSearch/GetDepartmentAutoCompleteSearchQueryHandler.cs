using AutoMapper;
using BSOFT.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSOFT.Application.Departments.Queries.GetDepartmentAutoComplete;


namespace BSOFT.Application.Departments.Queries.GetDepartmentAutoCompleteSearch
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
               DeptId = d.DeptId,
                DeptName = d.DeptName
            }).ToList();
            // Fetch departments from repository
            // var departments = await _departmentRepository.GetAllDepartmentAutoCompleteSearchAsync(request.SearchDept);
            // // Filter and project results
            // var filteredDepartments = departments
            //     .Where(d => string.IsNullOrEmpty(request.SearchDept) || d.DeptName.Contains(request.SearchDept))
            //     .Select(d => new DepartmentAutoCompleteVm
            //     {
            //         DeptId = d.DeptId,
            //         DeptName = d.DeptName
            //     })
            //     .ToList();

            // return filteredDepartments;
        }
        }
         
    }


