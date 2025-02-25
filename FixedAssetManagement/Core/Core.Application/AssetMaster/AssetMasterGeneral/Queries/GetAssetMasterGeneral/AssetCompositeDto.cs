using Core.Application.AssetLocation.Commands.CreateAssetLocation;
using Core.Application.AssetMaster.AssetLocation.Commands.UpdateAssetLocation;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetPurchase.Commands.CreateAssetPurchaseDetails;
using Core.Application.AssetMaster.AssetPurchase.Commands.UpdateAssetPurchaseDetails;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral
{
    public class AssetCompositeDto
    {
        public CreateAssetMasterGeneralCommand  AssetMaster { get; set; } = null!;
        public List<CreateAssetPurchaseDetailCommand>? AssetPurchaseDetails { get; set; }        
        public List<CreateAssetLocationCommand>? AssetLocation { get; set; }    

        public UpdateAssetMasterGeneralCommand UpdateAssetMaster { get; set; } = null!;
        public List<UpdateAssetPurchaseDetailCommand>? AssetPurchaseDetailsUpdate { get; set; }
        public List<UpdateAssetLocationCommand>? AssetLocationUpdate { get; set; }       
    }
}