using MediatR;

namespace Core.Application.Budget.Queries.GetBudgetById
{
    public class GetBudgetByIdQuery : IRequest<BudgetResponseDto>
    {
        public int BudgetId { get; set; }
    }
}
