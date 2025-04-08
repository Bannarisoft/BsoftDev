using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class PreventiveSchedulerItems
    {
        public int Id { get; set; }
        public int PreventiveSchedulerHdrId { get; set; }
        public PreventiveSchedulerHdr PreventiveScheduler { get; set; }
        public int ItemId { get; set; }
        public int RequiredQty { get; set; }
        public string OldItemId { get; set; }
    }
}