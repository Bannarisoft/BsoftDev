using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PreventiveSchedulers.Queries.GetPreventiveSchedulerById
{
    public class PreventiveSchedulerItemByIdDto
    {
        public int Id { get; set; }
        public int PreventiveSchedulerHdrId { get; set; }
        public string OldItemId { get; set; }
        public int RequiredQty { get; set; }
    }
}