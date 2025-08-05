using Core.Application.Budget.Queries.GetBudgetLogs;

namespace Core.Application.Common.Interfaces.Budget
{
    public interface IBudgetLogQueryRepository
    {
        Task<List<BudgetLogDto>> GetLogsAsync(int? budgetId, int? budgetDetailId);
    }
}
