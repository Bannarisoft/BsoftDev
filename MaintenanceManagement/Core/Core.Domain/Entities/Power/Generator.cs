using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities.Power
{
    public class Generator : BaseEntity
    {

        public string? Code { get; set; }
        public string? GenSetName { get; set; }
        public int UnitId { get; set; }
        public int Serialnumber { get; set; }
        public decimal KVA { get; set; }
        public decimal Current { get; set; }
        public decimal Voltage { get; set; }
        public decimal Power { get; set; }
        public int RPM { get; set; }
        public decimal PowerFactor { get; set; }
        public int MultiplicationFactor { get; set; }
        public decimal Frequency { get; set; }
        public decimal FuelTankCapacity { get; set; }
        public int GensetStatus { get; set; }     // working , under repair , not working

        public MiscMaster? GensetStatusType { get; set; }
        
        

    }
}