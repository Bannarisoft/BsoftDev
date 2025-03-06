using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetTransfer.Queries.GetAssetTransfered;

namespace Core.Application.Common.Mappings.AssetMaster
{
    public class AssetTransferProfile : Profile
    
    {
        public AssetTransferProfile()
        {

             CreateMap<Core.Domain.Entities.AssetMaster.AssetTransfer, AssetTransferDto>(); 
            
        }
        
    }
}