using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.RackMaster.Command.CreateRackMaster;
using Core.Application.RackMaster.Command.UpdateRackMaster;
using Core.Application.RackMaster.Queries.GetAllRackMaster;

namespace Core.Application.Common.Mappings
{
    public class RackMasterProfile : Profile
    {

        public RackMasterProfile()
        {
            CreateMap<Core.Domain.Entities.RackMaster, RackMasterDto>();

            CreateMap<CreateRackMasterCommand, Core.Domain.Entities.RackMaster>();

            CreateMap<UpdateRackMasterCommand, Core.Domain.Entities.RackMaster>()
            .ForMember(d => d.IsActive,     opt => opt.MapFrom(s => s.IsActive == 1
            ? Core.Domain.Common.BaseEntity.Status.Active : Core.Domain.Common.BaseEntity.Status.Inactive))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            
          //  CreateMap<UpdateRackMasterCommand, Core.Domain.Entities.RackMaster>();

            // CreateMap<DeleteRackMasterCommand, Core.Domain.Entities.RackMaster>();
        }
        
    }
}