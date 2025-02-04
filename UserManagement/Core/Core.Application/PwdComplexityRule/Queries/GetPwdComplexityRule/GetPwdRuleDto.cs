using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Application.PwdComplexityRule.Queries.GetPwdComplexityRule
{
    public class GetPwdRuleDto   {
        

         public int Id { get; set; }
       
        public string ? PwdComplexityRule  { get; set; } 

        public Status  IsActive { get; set; }
        public IsDelete IsDeleted { get; set; }

    }
}