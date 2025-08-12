using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class PartyAddress
    {
        public int Id { get; set; }
        public int PartyId { get; set; }
        public PartyMaster PartyAddressId { get; set; } = null!;
        public string? AddressType { get; set; } // Billing, Shipping, etc.
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
   
 
    }
}