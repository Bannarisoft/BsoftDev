using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered
{
    public class AssetTransferIssueDtlDto
    {
    public int AssetId { get; set; }
    public decimal AssetValue { get; set; }
    }
}