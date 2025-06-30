using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.MachineMaster.Command.CreateMachineMaster;
using Core.Application.MachineMaster.Command.DeleteMachineMaster;
using Core.Application.MachineMaster.Command.UpdateMachineMaster;
using Core.Application.MachineMaster.Queries.GetMachineMaster;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class MachineMasterProfile :Profile
    {
        public MachineMasterProfile()
        {
            CreateMap<Core.Domain.Entities.MachineMaster, MachineMasterDto>()
              ;

            CreateMap<Core.Domain.Entities.MachineMaster, MachineMasterAutoCompleteDto>()
             .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.MachineGroup.DepartmentId));
            CreateMap<CreateMachineMasterCommand, Core.Domain.Entities.MachineMaster>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsProductionMachine, opt => opt.MapFrom(src => src.IsProductionMachine == 1 ? true : false))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

            CreateMap<UpdateMachineMasterCommand, Core.Domain.Entities.MachineMaster>()
                .ForMember(dest => dest.IsProductionMachine, opt => opt.MapFrom(src => src.IsProductionMachine == 1 ? true : false))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));

               CreateMap<DeleteMachineMasterCommand, Core.Domain.Entities.MachineMaster>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted)); 

            
        }
    }
}