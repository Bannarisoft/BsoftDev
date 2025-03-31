using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneralById
{
    public class AssetSpecDTO
    {
        public int Id { get; set; }
        public string? SpecificationName { get; set; }
        public string? SpecificationValue { get; set; }
        public int SpecificationId { get; set; }
    }
}