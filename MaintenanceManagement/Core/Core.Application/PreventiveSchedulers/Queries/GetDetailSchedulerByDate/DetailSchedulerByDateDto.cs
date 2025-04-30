using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PreventiveSchedulers.Queries.GetDetailSchedulerByDate
{
    public class DetailSchedulerByDateDto
    {
        public int Id { get; set; }
        public int MachineGroupId { get; set; }
        public string GroupName { get; set; }
        public int MachineId { get; set; }
        public string MachineName { get; set; }
    }
}