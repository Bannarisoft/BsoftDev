using MediatR;

namespace Core.Application.Item.PutAway.Commands.CreatePutAwayRule
{
   public sealed record CreatePutAwayRuleCommand(CreatePutAwayRuleRequest Body) : IRequest<int>;
}