using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneralById
{
    public class AssetMasterDTO
    {
        public string? AssetName { get; set; }
        public string? AssetCode { get; set; }
        public int Quantity { get; set; }
        public string? UOMName { get; set; }
        public string? GroupName { get; set; }
        public string? CategoryName { get; set; }
        public string? SubCategoryName { get; set; }
        public string? AssetGroupId { get; set; }
        public string? AssetImage { get; set; }
        public AssetParentDTO? AssetParent { get; set; }
        public AssetLocationDTO? AssetLocation { get; set; }
        public IList<AssetPurchaseDetailDTO>? AssetPurchaseDetails { get; set; }
        public IList<AssetSpecDTO>? AssetSpecification { get; set; }
        public IList<AssetWarrantyDTOById>? AssetWarranty { get; set; }
        public IList<AssetAMCDTOById>? AssetAmc { get; set; }
        public AssetDisposalByIdDTO? AssetDisposal { get; set; }
        public IList<AssetInsuranceByIdDTO>? AssetInsurance { get; set; }
    }
}