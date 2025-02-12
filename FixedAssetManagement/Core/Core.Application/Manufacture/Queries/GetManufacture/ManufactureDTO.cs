using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Mappings;
using Core.Domain.Entities;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Manufacture.Queries.GetManufacture
{
    public class ManufactureDTO : IMapFrom<Manufactures>
    {
        public int Id { get; set; }
        public string? Code { get; set; }        
        public string? Name { get; set; }                
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
        public Status IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset?  CreatedAt { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedIP { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTimeOffset?  ModifiedAt { get; set; }
        public string? ModifiedByName { get; set; }
        public string? ModifiedIP { get; set; }   
        
    }
}