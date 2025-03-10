using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetTransfer.Queries.GetAssetTransfered
{
    public class AssetTransferDto
    {

         public int assetId { get; set; }         
        public int AssetCategoryId { get; set; }
        public string? AssetName { get; set; }
        public int  UnitId { get; set; }
        public int DepartmentId { get; set; }
        public int CustodianId { get; set; }
        public int UserID { get; set; }
        public int LocationId { get; set; }
        public int SubLocationId { get; set; }
        public int assetTransferId { get; set; }
        
    }
}