using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetDtlToTransfer
{
    public class GetAssetDetailsToTransferDto
    {
      public int AssetTransferId { get; set;}
      public int  AssetId { get; set;} 
      public decimal AssetValue { get; set;}  
                    
    }
}