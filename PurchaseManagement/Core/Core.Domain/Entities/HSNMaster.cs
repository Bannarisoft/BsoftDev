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
        public string? GstCategory { get; set; }  

        private decimal _gstPercentage;

        public decimal GstPercentage
        {
            get => _gstPercentage;
            set
            {
                _gstPercentage = value;
                CgstPercentage = Math.Round(value / 2, 2);
                SgstPercentage = Math.Round(value / 2, 2);
            }
        }

        public decimal CgstPercentage { get; private set; }
        public decimal SgstPercentage { get; private set; }
        public decimal IgstPercentage { get; set; }     
        public DateTimeOffset ValidFrom { get; set; } 
    }
}