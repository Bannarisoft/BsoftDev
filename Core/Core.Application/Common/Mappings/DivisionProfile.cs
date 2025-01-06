using AutoMapper;
using Core.Application.Divisions.Commands.CreateDivision;
using Core.Application.Divisions.Commands.DeleteDivision;
using Core.Application.Divisions.Commands.UpdateDivision;
using Core.Application.Divisions.Queries.GetDivisions;
using Core.Domain.Entities;

namespace Core.Application.Common.Mappings
{
    public class DivisionProfile : Profile
    {
        public DivisionProfile()
        {
            CreateMap<CreateDivisionCommand, Division>();
            CreateMap<UpdateDivisionCommand, Division>();
            CreateMap<DeleteDivisionCommand, Division>();
            CreateMap<Division, DivisionDTO>();
        }
    }
}