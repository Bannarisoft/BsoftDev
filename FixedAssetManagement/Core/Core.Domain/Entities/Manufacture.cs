using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class Manufactures  : BaseEntity
    {      
        public string? Code { get; set; }        
        public string? ManufactureName { get; set; }                
        public string? Type { get; set; }
        public string? Country { get; set; }        
        public string? State { get; set; }        
        public string? City { get; set; }        
        public string? AddressLine1 { get; set; }        
        public string? AddressLine2 { get; set; }        
        public int? PinCode { get; set; }        
        public string? PersonName { get; set; }        
        public int? PhoneNumber { get; set; }        
        public string? Email { get; set; }                
    }
}