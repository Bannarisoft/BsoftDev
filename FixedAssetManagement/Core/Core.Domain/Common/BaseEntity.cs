namespace Core.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public enum Status
        {
            Inactive = 0,
            Active  = 1
        }
        public enum IsDelete
        {
            NotDeleted = 0,
            Deleted = 1
        }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
      
        public string? CreatedByName { get; set; }
      
        public string? CreatedIP { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
       
        public string? ModifiedByName { get; set; }
       
        public string? ModifiedIP { get; set; }
        
    }
}