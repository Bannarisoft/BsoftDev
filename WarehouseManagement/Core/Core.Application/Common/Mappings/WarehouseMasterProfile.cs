using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.WarehouseMaster.GetAllWarehouseMaster;

namespace Core.Application.Common.Mappings
{
    public class WarehouseMasterProfile : Profile
    {
        
        public WarehouseMasterProfile()
        {
             CreateMap<Core.Domain.Entities.WarehouseMaster, WarehouseMasterDto>();
        }
    }
}