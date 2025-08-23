using MediatR;

namespace Core.Application.Item.PutAway.Commands.DeletePutAwayRule
{    
    public class DeletePutAwayRuleCommand : IRequest<int> 
    {
        public int Id { get; set; }
    }
}