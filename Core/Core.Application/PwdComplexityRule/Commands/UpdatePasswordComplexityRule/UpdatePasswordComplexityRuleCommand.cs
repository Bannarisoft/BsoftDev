using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.PwdComplexityRule.Queries;
using MediatR;

namespace Core.Application.PasswordComplexityRule.Commands.UpdatePasswordComplexityRule
{
    public class UpdatePasswordComplexityRuleCommand : IRequest<PwdRuleDto>
    {
        
        public int Id { get; set; }
       
        public string PwdComplexityRule  { get; set; }

        public byte IsActive { get; set; }


    }
}