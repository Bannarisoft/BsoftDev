using MediatR;

namespace Core.Application.Budget.Queries.GetAllBudgets
{
    public class GetAllBudgetsQuery : IRequest<List<BudgetListDto>>
    {        
        public int? FiscalYear { get; set; }
    }
}
