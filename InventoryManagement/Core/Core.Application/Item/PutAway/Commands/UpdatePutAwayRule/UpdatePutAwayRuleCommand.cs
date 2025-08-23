using Core.Application.Item.PutAway.Commands.CreatePutAwayRule;
using MediatR;

namespace Core.Application.Item.PutAway.Commands.UpdatePutAwayRule
{
    public sealed record UpdatePutAwayRuleCommand(int Id, CreatePutAwayRuleRequest Body) : IRequest<Unit>;
}