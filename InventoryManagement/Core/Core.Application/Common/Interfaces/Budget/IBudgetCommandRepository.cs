using Core.Domain.Entities.Budget;

namespace Core.Application.Common.Interfaces.Budget
{
    public interface IBudgetCommandRepository
    {
       Task<int> CreateBudgetAsync(BudgetMaster budgetMaster);
        Task<int> UpdateBudgetMasterAsync(int budgetId, decimal newAmount, string remarks);
        Task<int> UpdateBudgetDetailAsync(int detailId, decimal newAmount, string remarks);
        Task<bool> ExistsAsync(int budgetGroupId, int fiscalYear);
    }
}
