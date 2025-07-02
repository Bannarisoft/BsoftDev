using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetDtlToTransfer
{
    public class GetAssetDetailsToTransferHdrDto
    {
    public int AssetID { get; set; }
    public DateTimeOffset? DocDate { get; set; }
    public string? CategoryName { get; set; }
    public string? AssetCode { get; set; }
    public string? AssetName { get; set; }
    public int UnitId { get; set; }
    public string? UnitName { get; set; }
    public int LocationId { get; set; }
    public string? LocationName { get; set; }
    public int SubLocationId { get; set; }
    public string?  SubLocationName { get; set; }
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; }  
    public int  FromCustodianId { get; set; }
    public string? FromCustodianName { get; set; }

    public int? ToCustodianId { get; set; }
    public string? ToCustodianName { get; set;}

    public string? OldUnitId { get; set; }


     public List<GetAssetDetailsToTransferDto>? GetAssetDetailToTransfer { get; set; } 
    }
}