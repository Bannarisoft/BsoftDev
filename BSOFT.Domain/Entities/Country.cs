using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Domain.Common;

namespace BSOFT.Domain.Entities
{
    public class Countries  : BaseEntity
    {
        public int Id { get; set; }
        public string CountryCode { get; set; }=string.Empty;
        public string CountryName { get; set; }=String.Empty;               
       
    }
}