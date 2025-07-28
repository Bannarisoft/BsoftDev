using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class HSNMaster : BaseEntity
    {
        public int Type { get; set; }         
        public string? HSNCode { get; set; }      
        public string? Description { get; set; }  
        public string? GSTCategory { get; set; }  
        private decimal _gstPercentage;
        public decimal GSTPercentage
        {
            get => _gstPercentage;
            set
            {
                _gstPercentage = value;
                CGSTPercentage = Math.Round(value / 2, 2);
                SGSTPercentage = Math.Round(value / 2, 2);
            }
        }
        public decimal CGSTPercentage { get; private set; }
        public decimal SGSTPercentage { get; private set; }
        public decimal IGSTPercentage { get; set; }     
        public DateTimeOffset ValidFrom { get; set; } 
    }
}