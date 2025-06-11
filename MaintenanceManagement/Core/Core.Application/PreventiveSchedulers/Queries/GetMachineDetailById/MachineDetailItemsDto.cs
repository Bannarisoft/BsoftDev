using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PreventiveSchedulers.Queries.GetMachineDetailById
{
    public class MachineDetailItemsDto
    {
        public string OldItemId { get; set; }
        public string OldCategoryDescription { get; set; }
        public string OldGroupName { get; set; }
    }
}