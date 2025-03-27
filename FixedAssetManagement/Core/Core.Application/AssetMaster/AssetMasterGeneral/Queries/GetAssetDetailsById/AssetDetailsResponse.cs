using Core.Application.AssetLocation.Queries.GetAssetLocation;
using Core.Application.AssetMaster.AssetAdditionalCost.Queries.GetAssetAdditionalCost;
using Core.Application.AssetMaster.AssetAmc.Queries.GetAssetAmc;
using Core.Application.AssetMaster.AssetDisposal.Queries.GetAssetDisposal;
using Core.Application.AssetMaster.AssetInsurance.Queries.GetAssetInsurance;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetPurchase;
using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetDetailsById
{
    public class AssetDetailsResponse
    {
        public AssetMasterGeneralDTO? AssetMaster { get; set; }
        public AssetLocationDto? AssetLocation { get; set; }
        public AssetPurchaseDetailsDto? AssetPurchase { get; set; }
        public AssetAmcDto? AssetAMC { get; set; }
        public AssetWarrantyDTO? AssetWarranty { get; set; }
        public AssetSpecificationDTO? AssetSpecification { get; set; }
        public AssetDisposalDto? AssetDisposal { get; set; }
        public GetAssetInsuranceDto? AssetInsurance { get; set; }
        public AssetAdditionalCostDto? AssetAdditionalCost { get; set; }
    }    
}