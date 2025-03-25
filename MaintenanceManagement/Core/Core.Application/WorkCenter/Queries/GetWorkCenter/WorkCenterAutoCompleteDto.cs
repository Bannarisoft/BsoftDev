using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.WorkCenter.Queries.GetWorkCenter
{
    public class WorkCenterAutoCompleteDto
    {
        public int Id { get; set; }
        public string? WorkCenterName { get; set; }
    }
}