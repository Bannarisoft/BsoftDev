using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PreventiveSchedulers.Commands.UpdatePreventiveScheduler
{
    public class PreventiveSchedulerItemUpdateDto
    {
        public int PreventiveSchedulerHeaderId { get; set; }
        public string? ItemId { get; set; }
        public int RequiredQty { get; set; }
    }
}