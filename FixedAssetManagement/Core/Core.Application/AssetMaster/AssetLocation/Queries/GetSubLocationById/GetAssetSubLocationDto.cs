using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetLocation.Queries.GetSubLocationById
{
    public class GetAssetSubLocationDto
    {

        public int Id { get; set; }
        public string? Code { get; set; }
        public string? SubLocationName { get; set; }
        public string? Description { get; set; }
        public int UnitId { get; set; }
        public string? DepartmentId { get; set; }
        public string? LocationId { get; set; }                
       
                


    }
}