using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral
{
    public interface IAssetMasterGeneralQueryRepository
    {
        Task<AssetMasterGeneralDTO>  GetByIdAsync(int assetId);
        Task<AssetMasterGeneralDTO>  GetByParentIdAsync(int assetTypeId);
        Task<(List<AssetMasterGeneralDTO>,int)> GetAllAssetAsync(int PageNumber, int PageSize, string? SearchTerm);        
        Task<List<AssetMasterGeneralDTO>> GetByAssetNameAsync(string assetName);    
        Task<List<Core.Domain.Entities.MiscMaster>> GetAssetTypeAsync();     
        Task<List<Core.Domain.Entities.MiscMaster>> GetWorkingStatusAsync();       
        Task<bool> GetAssetChildDetails(int assetId);
        Task<string?> GetLatestAssetCode(int assetGroupId, int assetCategoryId,int DepartmentId,int LocationId);
        Task<string> GetBaseDirectoryAsync();
        Task<List<Core.Domain.Entities.MiscMaster>> GetAssetPattern();
        Task<(dynamic AssetResult, dynamic LocationResult, IEnumerable<dynamic> PurchaseDetails, IEnumerable<dynamic> Spec, IEnumerable<dynamic> Warranty,IEnumerable<dynamic> Amc,dynamic Disposal, IEnumerable<dynamic> Insurance , IEnumerable<dynamic> AdditionalCost)> GetAssetMasterByIdAsync(int assetId);
        Task<(dynamic AssetResult, dynamic LocationResult, IEnumerable<dynamic> PurchaseDetails,  IEnumerable<dynamic> AdditionalCost)> GetAssetMasterSplitByIdAsync(int assetId);
        Task<(string CompanyName, string UnitName)> GetCompanyUnitAsync(int companyId,int unitId);      
    }
}