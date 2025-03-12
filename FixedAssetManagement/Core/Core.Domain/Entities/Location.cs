using Core.Domain.Common;
using Core.Domain.Entities.AssetMaster;


namespace Core.Domain.Entities
{
  
    public class Location : BaseEntity
    {
         
        public string? Code { get; set; }
        public string? LocationName { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public int UnitId { get; set; }
        public int DepartmentId { get; set; }

        public int SubLocationId { get; set; }
    
        

         // One-to-Many Relationship with SubLocation
        public ICollection<SubLocation> SubLocations { get; set; } = new List<SubLocation>();

        public ICollection<AssetLocation>? AssetLocations { get; set; }

        public ICollection<AssetTransferReceiptDtl>? AssetTransferReceiptLocation { get; set; }



    }
       
   
}