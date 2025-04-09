using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler
{
    public class PreventiveSchedulerDtlDto
    {
        public int MachineId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly NextDueDate { get; set; }
    }
}