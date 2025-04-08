using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class PreventiveSchedulerActivity
    {
        public int Id { get; set; }
        public int PreventiveSchedulerHdrId { get; set; }
        public PreventiveSchedulerHdr PreventiveScheduler { get; set; }
        public int ActivityId { get; set; }
        public ActivityMaster Activity { get; set; }
        public decimal EstimatedTimeHrs { get; set; }
        public string Description { get; set; }
    }
}