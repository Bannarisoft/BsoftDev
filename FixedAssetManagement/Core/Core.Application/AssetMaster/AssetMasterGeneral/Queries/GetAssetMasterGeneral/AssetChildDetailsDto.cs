

using Core.Application.Common.Mappings;
using Core.Domain.Entities;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral
{
    public class AssetChildDetailsDto : IMapFrom<AssetMasterGenerals>
    {
        public int Id { get; set; }
        public string? AssetCode { get; set; }
        public string? AssetName { get; set; }
        public int AssetLocation { get; set; }
        public int AssetPurchase { get; set; }
        public int AssetWarranty { get; set; }
        public int AssetSpec { get; set; }
        public int AssetAmc { get; set; }
        public int AssetAdditionalCost { get; set; }
        public int AssetInsurance { get; set; }
        public int AssetDisposal { get; set; } 
        public int AssetDepreciation { get; set; } 
    }
}
