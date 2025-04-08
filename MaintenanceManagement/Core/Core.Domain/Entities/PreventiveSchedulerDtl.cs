using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class PreventiveSchedulerDtl
    {
        public int Id { get; set; }
        public int PreventiveSchedulerId { get; set; }
        public PreventiveSchedulerHdr PreventiveScheduler { get; set; }
        public int MachineId { get; set; }
        public MachineMaster Machine { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly NextDueDate { get; set; }
    }
}