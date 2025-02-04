using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Mappings;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Application.TimeZones.Queries.GetTimeZones
{
    public class TimeZonesDto 
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; } // New Field
        public string? Offset { get; set; } // New Field (e.g., UTC+05:30)
        public Status  IsActive { get; set; }
        public IsDelete IsDeleted { get; set; }
    }
}