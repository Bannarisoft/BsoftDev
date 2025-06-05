using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities.Power
{
    public class FeederGroup : BaseEntity
    {
       
        public string? FeederGroupCode { get; set; }

        public string? FeederGroupName { get; set; }

        
    }
}