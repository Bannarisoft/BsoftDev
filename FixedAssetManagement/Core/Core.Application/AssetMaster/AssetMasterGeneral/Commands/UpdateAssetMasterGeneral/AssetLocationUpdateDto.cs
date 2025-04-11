using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetMasterGeneral
{
    public class AssetLocationUpdateDto
    {
        public int Id { get; set; }
        public int AssetId { get; set; } 
        public int UnitId { get; set; } 
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }
        public int SubLocationId { get; set; } 
        public int CustodianId { get; set; }
        public int UserId { get; set; } 
    }
}