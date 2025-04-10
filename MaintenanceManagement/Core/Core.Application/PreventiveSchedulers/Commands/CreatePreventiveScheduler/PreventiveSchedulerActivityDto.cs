using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler
{
    public class PreventiveSchedulerActivityDto
    {
        public int ActivityId { get; set; }
        public decimal EstimatedTimeHrs { get; set; }
        public string? Description { get; set; }
    }
}