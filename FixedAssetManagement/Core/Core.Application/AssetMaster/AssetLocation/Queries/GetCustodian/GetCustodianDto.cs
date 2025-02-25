using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetLocation.Queries.GetCustodian
{
    public class GetCustodianDto
    {
        public int CustodianId { get ; set; }
        public string? CustodianName { get; set; } 
    }
}