using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.WarehouseMaster.Command.CreateWarehouseMaster;
using Core.Application.WarehouseMaster.Command.UpdateWarehouseMaster;
using Core.Application.WarehouseMaster.GetAllWarehouseMaster;

namespace Core.Application.Common.Mappings
{
    public class WarehouseMasterProfile : Profile
    {

        public WarehouseMasterProfile()
        {
            CreateMap<Core.Domain.Entities.WarehouseMaster, WarehouseMasterDto>();

            CreateMap<CreateWarehouseMasterCommand, Core.Domain.Entities.WarehouseMaster>()
                .ForMember(dest => dest.AllowedItemGroups, opt => opt.Ignore()); 
                
            CreateMap<UpdateWarehouseMasterCommand, Core.Domain.Entities.WarehouseMaster>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());    
        }
    }
}