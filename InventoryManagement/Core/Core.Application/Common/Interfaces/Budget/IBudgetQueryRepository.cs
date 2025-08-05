using Core.Application.Budget.Queries.GetAllBudgets;
using Core.Application.Budget.Queries.GetBudgetById;

namespace Core.Application.Common.Interfaces.Budget
{
    public interface IBudgetQueryRepository
    {
        Task<BudgetResponseDto?> GetBudgetByIdAsync(int budgetId);
         Task<List<BudgetListDto>> GetAllBudgetsAsync(int? fiscalYear);
    }
}
