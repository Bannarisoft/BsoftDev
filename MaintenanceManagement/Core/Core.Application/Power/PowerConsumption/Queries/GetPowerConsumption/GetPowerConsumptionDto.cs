using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Power.PowerConsumption.Queries.GetPowerConsumption
{
    public class GetPowerConsumptionDto
    {
        public int Id { get; set; }
        public int FeederTypeId { get; set; }
        public string? FeederType { get; set; }
        public int FeederId { get; set; }
        public string? FeederName { get; set; }
        public int UnitId { get; set; }
        public string? UnitName { get; set; }
        public decimal OpeningReading { get; set; }
        public decimal ClosingReading { get; set; }
        public decimal TotalUnits { get; set; }
        public Status IsActive { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string? CreatedByName { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public string? ModifiedByName { get; set; }

    }
}