using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PreventiveSchedulers.Commands.UpdatePreventiveScheduler
{
    public class PreventiveSchedulerActivityUpdateDto
    {
        public int PreventiveSchedulerHdrId { get; set; }
        public int ActivityId { get; set; }
        public decimal EstimatedTimeHrs { get; set; }
        public string? Description { get; set; }
    }
}