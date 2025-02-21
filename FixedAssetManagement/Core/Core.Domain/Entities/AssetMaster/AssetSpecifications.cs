using Core.Domain.Common;

namespace Core.Domain.Entities.AssetMaster
{
    public class AssetSpecifications : BaseEntity
    {
         public int AssetId { get; set; } 
         public AssetMasterGenerals AssetMasterId { get; set; } = null!;
         public int? ManufactureId { get; set; } 
         public Manufactures Manufacture { get; set; } = null!;
         public DateTimeOffset? ManufactureDate { get; set; } 
         public int SpecificationId { get; set; } 
         public SpecificationMasters SpecificationMaster { get; set; } = null!;
         public string? SpecificationValue { get; set; } 
         public string? SerialNumber { get; set; } 
         public string? ModelNumber { get; set; } 
    }
}