using Core.Domain.Common;


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

    }
       
   
}