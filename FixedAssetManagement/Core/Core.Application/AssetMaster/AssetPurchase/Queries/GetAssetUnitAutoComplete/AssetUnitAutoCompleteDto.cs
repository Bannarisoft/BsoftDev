using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetPurchase
{
    public class AssetUnitAutoCompleteDto
    {
        public int OldUnitId { get; set; }
        public string? UnitName { get; set; }
    }
}