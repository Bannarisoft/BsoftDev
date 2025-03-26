using System.Data;
using Core.Application.AssetLocation.Queries.GetAssetLocation;
using Core.Application.AssetMaster.AssetAdditionalCost.Queries.GetAssetAdditionalCost;
using Core.Application.AssetMaster.AssetAmc.Queries.GetAssetAmc;
using Core.Application.AssetMaster.AssetDisposal.Queries.GetAssetDisposal;
using Core.Application.AssetMaster.AssetInsurance.Queries.GetAssetInsurance;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetPurchase;
using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Dapper;

namespace FAM.Infrastructure.Repositories.AssetMaster.AssetMasterGeneral
{
    public class AssetDetailsQueryRepository : IAssetDetailsQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        public AssetDetailsQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }     
         public async Task<AssetMasterGeneralDTO> GetAssetMasterByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<AssetMasterGeneralDTO>(
                " SELECT * FROM FixedAsset.AssetMaster WHERE Id = @AssetId  and Isdeleted=0",                 new { AssetId = assetId });
        }

        public async Task<AssetLocationDto> GetAssetLocationByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<AssetLocationDto>(
                "SELECT * FROM FixedAsset.AssetLocation WHERE AssetId = @AssetId ", new { AssetId = assetId });
        }

        public async Task<AssetPurchaseDetailsDto> GetAssetPurchaseByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<AssetPurchaseDetailsDto>(
                "SELECT * FROM FixedAsset.AssetPurchaseDetails WHERE AssetId = @AssetId ", new { AssetId = assetId });
        }

        public async Task<AssetAmcDto> GetAssetAMCByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<AssetAmcDto>(
                "SELECT * FROM FixedAsset.AssetAmc WHERE AssetId = @AssetId and Isdeleted=0" , new { AssetId = assetId });
        }

        public async Task<AssetWarrantyDTO> GetAssetWarrantyByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<AssetWarrantyDTO>(
                "SELECT * FROM FixedAsset.AssetWarranty WHERE AssetId = @AssetId and Isdeleted=0", new { AssetId = assetId });
        }

        public async Task<AssetSpecificationDTO> GetAssetSpecificationByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<AssetSpecificationDTO>(
                "SELECT * FROM FixedAsset.AssetSpecifications WHERE AssetId = @AssetId and Isdeleted=0", new { AssetId = assetId });
        }

        public async Task<AssetDisposalDto> GetAssetDisposalByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<AssetDisposalDto>(
                "SELECT * FROM FixedAsset.AssetDisposal WHERE AssetId = @AssetId and Isdeleted=0", new { AssetId = assetId });
        }
    public async Task<GetAssetInsuranceDto> GetAssetInsuranceByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<GetAssetInsuranceDto>(
                "SELECT * FROM FixedAsset.AssetDisposal WHERE AssetId = @AssetId and Isdeleted=0", new { AssetId = assetId });
        }
            public async Task<AssetAdditionalCostDto> GetAssetAdditionalCostByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<AssetAdditionalCostDto>(
                "SELECT * FROM FixedAsset.AssetAdditionalCost WHERE AssetId = @AssetId ", new { AssetId = assetId });
        }
    }    
}