using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Contracts.Dtos.Party
{
    public class PartyDto
    {
        public int PartyId { get; set; }
        public string PartyCode { get; set; } 
        public string PartyName { get; set; }
        public int RegistrationTypeId { get; set; }
        public string GSTNumber { get; set; }
        public string GSTStateCode { get; set; }
        public string PAN { get; set; }
        public string TAN { get; set; }
        public string MSMENO { get; set; }
        public int IsTDSApplicable { get; set; }
        public int IsTCSApplicable { get; set; }
        public int IsGstReverseCharge { get; set; }
        public int CreditDays { get; set; }        
        public string PartyStatus { get; set; }
    }
}