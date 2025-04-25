using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class PreventiveSchedulerItems
    {
        public int Id { get; set; }
        public int PreventiveSchedulerHeaderId { get; set; }
        public required PreventiveSchedulerHeader PreventiveScheduler { get; set; }
        public int ItemId { get; set; }
        public int RequiredQty { get; set; }
        public int? SourceId { get; set; }
        public string? OldItemId { get; set; }
        public string? OldCategoryDescription { get; set; }
        public string? OldGroupName{ get; set; }
    }
}