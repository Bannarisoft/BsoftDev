using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.WarehouseMaster.Queries.GetWareMasterAutoComplete
{
    public class GetWarehouseAutoCompleteDto  
    {
        public int  Id { get; set; }
        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; } 
        
    }
}