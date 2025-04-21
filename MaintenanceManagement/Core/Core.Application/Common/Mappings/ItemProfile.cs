using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Item.ItemGroup.Queries;
using Core.Application.Item.ItemMaster.Queries;

namespace Core.Application.Common.Mappings
{
    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<Core.Domain.Entities.ItemGroupCode, GetItemGroupDto>();
            CreateMap<Core.Domain.Entities.ItemMaster, GetItemMasterDto>();
          
        }
    }
}