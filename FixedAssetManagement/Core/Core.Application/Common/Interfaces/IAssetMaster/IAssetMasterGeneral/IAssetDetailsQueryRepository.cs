using Core.Application.AssetLocation.Queries.GetAssetLocation;
using Core.Application.AssetMaster.AssetAdditionalCost.Queries.GetAssetAdditionalCost;
using Core.Application.AssetMaster.AssetAmc.Queries.GetAssetAmc;
using Core.Application.AssetMaster.AssetDisposal.Queries.GetAssetDisposal;
using Core.Application.AssetMaster.AssetInsurance.Queries.GetAssetInsurance;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetPurchase;
using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral
{
    public interface IAssetDetailsQueryRepository
    {
        Task<AssetMasterGeneralDTO> GetAssetMasterByIdAsync(int assetId);
        Task<AssetLocationDto> GetAssetLocationByIdAsync(int assetId);
        Task<AssetPurchaseDetailsDto> GetAssetPurchaseByIdAsync(int assetId);
        Task<AssetAmcDto> GetAssetAMCByIdAsync(int assetId);
        Task<AssetWarrantyDTO> GetAssetWarrantyByIdAsync(int assetId);
        Task<AssetSpecificationDTO> GetAssetSpecificationByIdAsync(int assetId);
        Task<AssetDisposalDto> GetAssetDisposalByIdAsync(int assetId);
        Task<GetAssetInsuranceDto> GetAssetInsuranceByIdAsync(int assetId);
        Task<AssetAdditionalCostDto> GetAssetAdditionalCostByIdAsync(int assetId);
    }
}