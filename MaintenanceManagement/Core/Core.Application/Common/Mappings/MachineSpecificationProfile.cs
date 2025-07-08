using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.MachineSpecification.Command;
using Core.Application.MachineSpecification.Command.CreateMachineSpecfication;
using Core.Application.MachineSpecification.DeleteMachineSpecfication;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class MachineSpecificationProfile : Profile
    {
        public MachineSpecificationProfile()
        {
            CreateMap<Core.Domain.Entities.MachineSpecification, MachineSpecificationDto>();

            CreateMap<MachineSpecificationCreateDto, Core.Domain.Entities.MachineSpecification>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));


            CreateMap<DeleteMachineSpecficationCommand, Core.Domain.Entities.MachineSpecification>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted)); 
        }
    }
}