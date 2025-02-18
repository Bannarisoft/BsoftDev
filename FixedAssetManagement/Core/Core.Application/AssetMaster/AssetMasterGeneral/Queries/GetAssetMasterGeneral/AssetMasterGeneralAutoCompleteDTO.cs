

using Core.Application.Common.Mappings;
using Core.Domain.Entities;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral
{
    public class AssetMasterGeneralAutoCompleteDTO : IMapFrom<AssetMasterGenerals>
    {
        public int Id { get; set; }
        public string? AssetCode { get; set; }
        public string? AssetName { get; set; } 
    }
}