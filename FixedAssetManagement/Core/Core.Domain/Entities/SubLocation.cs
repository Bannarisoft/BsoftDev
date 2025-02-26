using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;
using Core.Domain.Entities.AssetMaster;

namespace Core.Domain.Entities
{
    public class SubLocation : BaseEntity
    {
        public string? Code { get; set; }
        public string? SubLocationName { get; set; }
        public string? Description { get; set; }
        public int UnitId { get; set; }
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }
        public Location? Location { get; set; } 
        //AssetSubLocation from AssetLocation
        public ICollection<AssetLocation>? AssetSubLocation{ get; set; } 



    }
}