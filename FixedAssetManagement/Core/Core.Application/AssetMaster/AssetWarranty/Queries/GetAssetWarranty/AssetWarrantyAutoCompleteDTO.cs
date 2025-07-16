using Core.Application.Common.Mappings;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty
{
    public class AssetWarrantyAutoCompleteDTO : IMapFrom<AssetWarranties>
    {
        public int Id { get; set; }        
        public string? AssetCode { get; set; } 
    }
}