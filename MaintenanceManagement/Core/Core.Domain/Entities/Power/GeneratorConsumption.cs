using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities.Power
{
    public class GeneratorConsumption : BaseEntity
    {

        public int GeneratorId { get; set; }
        public MachineMaster? GeneratorTran { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public decimal RunningHours { get; set; }
        public decimal DieselConsumption { get; set; }
        public decimal OpeningEnergyReading { get; set; }
        public decimal ClosingEnergyReading { get; set; }
        public decimal Energy { get; set; }
        public int UnitId { get; set; }
        public int? PurposeId { get; set; }    
        public MiscMaster? GensetPurposeType { get; set; }
    }
}