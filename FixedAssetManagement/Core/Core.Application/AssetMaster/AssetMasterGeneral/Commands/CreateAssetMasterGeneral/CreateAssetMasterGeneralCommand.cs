using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral
{
    public class CreateAssetMasterGeneralCommand : IRequest<ApiResponseDTO<AssetMasterGeneralDTO>>  
    {
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }   
        public int UnitId { get; set; }
        public string? UnitName { get; set; } 
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
    }
}