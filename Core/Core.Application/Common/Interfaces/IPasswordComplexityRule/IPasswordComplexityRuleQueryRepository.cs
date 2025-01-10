using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IPasswordComplexityRule
{
    public interface IPasswordComplexityRuleQueryRepository 
    {
        
      Task<List<Core.Domain.Entities.PasswordComplexityRule>> GetPasswordComplexityAsync();
      

    }
}