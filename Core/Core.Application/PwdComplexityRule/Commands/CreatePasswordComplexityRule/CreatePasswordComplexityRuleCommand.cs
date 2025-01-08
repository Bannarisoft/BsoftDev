using Core.Application.PwdComplexityRule.Queries;
using MediatR;

namespace Core.Application.PwdComplexityRule.Commands.CreatePasswordComplexityRule
{
    public class CreatePasswordComplexityRuleCommand : IRequest<PwdRuleDto>
    {

        public int Id { get; set; }
       
        public string PwdComplexityRule  { get; set; }

        public byte IsActive { get; set; }
        
    }
}