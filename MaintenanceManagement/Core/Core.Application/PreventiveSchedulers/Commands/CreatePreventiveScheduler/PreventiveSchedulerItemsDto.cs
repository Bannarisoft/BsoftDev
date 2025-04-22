using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler
{
    public class PreventiveSchedulerItemsDto
    {
        public string ItemId { get; set; }
        public int RequiredQty { get; set; }
        public int? SourceId { get; set; }
    }
}