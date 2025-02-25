using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetLocation.Commands.CreateAssetLocation;
using Core.Application.AssetLocation.Queries.GetAssetLocation;
using Core.Application.AssetMaster.AssetLocation.Commands.UpdateAssetLocation;
using Core.Application.AssetMaster.AssetLocation.Queries.GetCustodian;

namespace Core.Application.Common.Mappings
{
    public class AssetLocationProfile : Profile
    {

        public AssetLocationProfile()
        {
            CreateMap<Core.Domain.Entities.AssetMaster.AssetLocation,AssetLocationDto>();

            CreateMap<CreateAssetLocationCommand, Core.Domain.Entities.AssetMaster.AssetLocation>();
            CreateMap<UpdateAssetLocationCommand, Core.Domain.Entities.AssetMaster.AssetLocation>();

            CreateMap<Core.Domain.Entities.AssetMaster.Employee,GetCustodianDto>()           
            .ForMember(dest => dest.CustodianId, opt => opt.MapFrom(src => src.Empcode))
            .ForMember(dest => dest.CustodianName, opt => opt.MapFrom(src => src.Empname));
                
        }
        
    }
}