using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification
{
    public class AssetSpecificationJsonDto
    {
        public int AssetId { get; set; }
        public string? AssetName { get; set; }
        public string AssetCode { get; set; }
        public string? ManufactureName { get; set; }
        public Status IsActive { get; set; }
        public List<SpecificationDTO> Specifications { get; set; } = new List<SpecificationDTO>();
    }

    public class SpecificationDTO
    {
        public int SpecificationId { get; set; }
        public string? SpecificationName { get; set; }
        public string? SpecificationValue { get; set; }
        public string? SerialNumber { get; set; }
        public string? ModelNumber { get; set; }
    }
}