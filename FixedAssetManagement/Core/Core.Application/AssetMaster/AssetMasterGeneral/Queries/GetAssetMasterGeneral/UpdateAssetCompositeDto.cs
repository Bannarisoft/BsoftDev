using Core.Application.AssetMaster.AssetLocation.Commands.UpdateAssetLocation;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetPurchase.Commands.UpdateAssetPurchaseDetails;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral
{
    public class UpdateAssetCompositeDto
    {
        public UpdateAssetMasterGeneralCommand UpdateAssetMaster { get; set; } = null!;
        public List<UpdateAssetPurchaseDetailCommand>? UpdateAssetPurchaseDetail { get; set; }
        public List<UpdateAssetLocationCommand>? UpdateAssetLocation { get; set; }     
    }
}