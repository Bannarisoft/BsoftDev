
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using MediatR;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetMasterGeneral
{
    public class UpdateAssetMasterGeneralCommand : IRequest<ApiResponseDTO<AssetMasterGeneralDTO>>     
    {
        public int Id { get; set; }       
        public int CompanyId { get; set; }       
        public string? AssetCode { get; set; }        
        public string? AssetName { get; set; }                
        public int AssetGroupId { get; set; }        
        public int AssetCategoryId { get; set; }        
        public int AssetSubCategoryId { get; set; }        
        public int AssetParentId { get; set; }        
        public string? AssetType { get; set; }                
        public string? MachineCode { get; set; }   
        public int? Quantity { get; set; }
        public int? UOMId { get; set; }
        public string? AssetDescription { get; set; }
        public string? WorkingStatus { get; set; }
        public string? AssetImage { get; set; }
        public bool? NonDepreciated { get; set; }
        public bool? Tangible { get; set; }
        public Status IsActive { get; set; }
    }
}