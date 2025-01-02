using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class Countries  : BaseEntity
    {
        public int Id { get; set; }
        public string CountryCode { get; set; }=string.Empty;
        public string CountryName { get; set; }=String.Empty;               
        public ICollection<States> States { get; set; } = new List<States>();
    }
}