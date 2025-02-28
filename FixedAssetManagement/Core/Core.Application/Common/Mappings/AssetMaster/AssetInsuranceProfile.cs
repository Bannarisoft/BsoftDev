using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetInsurance.Commands.CreateAssetInsurance;
using Core.Application.AssetMaster.AssetInsurance.Queries.GetAssetInsurance;

namespace Core.Application.Common.Mappings.AssetMaster
{
    public class AssetInsuranceProfile : Profile
    {

        public AssetInsuranceProfile()
        {
            CreateMap<Core.Domain.Entities.AssetMaster.AssetInsurance, GetAssetInsuranceDto>();
             CreateMap<CreateAssetInsuranceCommand, Core.Domain.Entities.AssetMaster.AssetInsurance>();


        }
        
    }
}