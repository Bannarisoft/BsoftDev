using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities.Power
{
    public class PowerConsumption : BaseEntity
    {
        public int FeederTypeId { get; set; }
        public MiscMaster? FeederTypePower { get; set; }
        public int FeederId { get; set; }
        public Feeder? FeederPower { get; set; }
        public int UnitId { get; set; }
        public decimal OpeningReading { get; set; }
        public decimal ClosingReading { get; set; }
        public decimal TotalUnits { get; set; }
    }
}