using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PreventiveSchedulers.Commands.RescheduleBulkImport
{
    public class PreventiveSchedulerBulkImprotActivityDto
    {
        public int PreventiveSchedulerHeaderId { get; set; }
        public int ActivityId { get; set; }
    }
}