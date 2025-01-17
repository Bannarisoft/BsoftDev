using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class Cities : BaseEntity
    {
        public int Id { get; set; }
        public string CityName   { get; set; }=string.Empty;
        public string CityCode { get; set; }=string.Empty;
        public int StateId { get; set; }
        public States States { get; set; } 
        
    }
}