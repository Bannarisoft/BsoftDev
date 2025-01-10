using AutoMapper;
using Core.Application.Departments.Queries.GetDepartments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Application.Departments.Commands.CreateDepartment;
using Core.Application.Departments.Commands.DeleteDepartment;
using Core.Application.Departments.Commands.UpdateDepartment;

namespace Core.Application.Common.Mappings
{
    public class DepartmentProfile : Profile
    {
          public DepartmentProfile()
    {
      
            CreateMap<CreateDepartmentCommand, Department>();

            CreateMap<DeleteDepartmentCommand, Department>();

            CreateMap<Department, DepartmentDto>();
            CreateMap<DepartmentStatusDto, Department>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

           CreateMap<UpdateDepartmentCommand, Department>() ;                  

    }
    }
}