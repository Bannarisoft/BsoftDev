using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PwdComplexityRule.Queries
{
    public class PwdRuleDto
    {
         public int Id { get; set; }
       
          public string PwdComplexityRule  { get; set; }

          public byte IsActive { get; set; }

    }
}