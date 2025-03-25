using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Mappings;
using Core.Domain.Entities;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral
{
    public class AssetMasterImageDto : IMapFrom<AssetMasterGenerals>
    {
        public string? AssetImage { get; set; }
        public string? AssetImageBase64 { get; set; } 

    }
}