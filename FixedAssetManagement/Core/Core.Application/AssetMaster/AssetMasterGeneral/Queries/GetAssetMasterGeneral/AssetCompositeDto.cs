using Core.Application.AssetLocation.Commands.CreateAssetLocation;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetPurchase.Commands.CreateAssetPurchaseDetails;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral
{
    public class AssetCompositeDto
    {
        public CreateAssetMasterGeneralCommand  AssetMaster { get; set; } = null!;
        public List<CreateAssetPurchaseDetailCommand>? AssetPurchaseDetails { get; set; }        
        public List<CreateAssetLocationCommand>? AssetLocation { get; set; }       
    }
}