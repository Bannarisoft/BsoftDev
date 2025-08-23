using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.BinMaster.Command.CreateBinMaster;
using Core.Application.BinMaster.Command.UpdateBinMaster;
using Core.Application.BinMaster.Queries.GetAllBinMaster;

namespace Core.Application.Common.Mappings
{
    public class BinMasterProfile : Profile
    {

        public BinMasterProfile()
        {
            CreateMap<Core.Domain.Entities.BinMaster, BinMasterDto>();

            CreateMap<CreateBinMasterCommand, Core.Domain.Entities.BinMaster>();

            CreateMap<UpdateBinMasterCommand, Core.Domain.Entities.BinMaster>()
            .ForMember(d => d.IsActive,     opt => opt.MapFrom(s => s.IsActive == 1
            ? Core.Domain.Common.BaseEntity.Status.Active : Core.Domain.Common.BaseEntity.Status.Inactive));
            
           
        }
        
    }
}