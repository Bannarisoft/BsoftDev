using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Power.FeederGroup.Queries.GetFeederGroup
{
    public class FeederGroupDto
    {
          public int Id { get; set; }

         public string? FeederGroupCode { get; set; }  
        public string? FeederGroupName { get; set; }
        public int  UnitId { get; set; }
        public Status IsActive { get; set; }
    }
}