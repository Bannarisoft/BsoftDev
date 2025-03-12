using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered
{
    public class AssetTransferJsonDto
    {
    public int AssetTransferId { get; set; }
    public DateTimeOffset DocDate { get; set; }
    public int TransferType { get; set; }
    public int FromUnitId { get; set; }
    public int ToUnitId { get; set; }
    public int FromDepartmentId { get; set; }
    public int ToDepartmentId { get; set; }
    public int FromCustodianId { get; set; }
    public int ToCustodianId { get; set; }
     public string? Status { get; set; }
    public string? FromCustodianName { get; set; }
    public string? ToCustodianName { get; set; }
    public string? ModifiedIP { get; set; }
    public int ModifiedBy { get; set; }
    public string? ModifiedByName { get; set; }
    public DateTimeOffset ModifiedDate { get; set; }
    // public string CreatedIP { get; set; }
    // public string CreatedByName { get; set; }   
    public List<AssetTransferDetailJsonDto>? AssetTransferDetails { get; set; }
    }
}