using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PreventiveSchedulers.Queries.GetSchedulerByDate
{
    public class SchedulerByDateDto
    {
        public int TotalScheduleCount { get; set; }
        public string ScheduleDate { get; set; }
    }
}