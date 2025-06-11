using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities.Power
{
    public class Feeder : BaseEntity
    {
        public string? FeederCode { get; set; }
        public string? FeederName { get; set; }
        public int FeederGroupId { get; set; }
        public FeederGroup? FeederGroup { get; set; }
        public int FeederTypeId { get; set; }
        public MiscMaster? FeederType { get; set; }
        public int DepartmentId { get; set; }
        public string? Description { get; set; }
        public decimal? MultiplicationFactor { get; set; }
        public DateTimeOffset EffectiveDate { get; set; }
        public decimal? OpeningReading { get; set; }
        public bool HighPriority { get; set; }
        public decimal? Target { get; set; }
        public int UnitId { get; set; }  
        public int? ParentFeederId { get; set; }
        public Feeder? ParentFeeder { get; set; }
        public ICollection<Feeder>? SubFeeders { get; set; }      
        
    }
}