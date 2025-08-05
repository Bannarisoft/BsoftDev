using Core.Application.Budget.Queries.GetBudgetLogs;
using Core.Application.Common.Interfaces.Budget;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class BudgetLogQueryRepository : IBudgetLogQueryRepository
    {
        private readonly ApplicationDbContext _context;

        public BudgetLogQueryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BudgetLogDto>> GetLogsAsync(int? budgetId, int? budgetDetailId)
        {
             var query = _context.BudgetLog
                .Include(l => l.BudgetDetail)
                .AsQueryable();

            if (budgetDetailId.HasValue)
                query = query.Where(l => l.BudgetDetailId == budgetDetailId.Value);

            if (budgetId.HasValue)
                query = query.Where(l => l.BudgetDetail.BudgetId == budgetId.Value);

            return await query
                .OrderByDescending(l => l.CreatedDate)   // ✅ Order on entity property
                .Select(log => new BudgetLogDto
                {
                    Id = log.Id,
                    BudgetDetailId = log.BudgetDetailId,
                    ActionTypeId = log.ActionTypeId,
                    OldBudgetAmount = log.OldBudgetAmount,
                    NewBudgetAmount = log.NewBudgetAmount,
                    Remarks = log.Remarks             
                })
                .ToListAsync();
        }
    }
}
