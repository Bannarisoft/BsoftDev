namespace Core.Application.Budget.Queries.GetBudgetLogs
{
    public class BudgetLogDto
    {
        public int Id { get; set; }
        public int BudgetDetailId { get; set; }
        public int ActionTypeId { get; set; }
        public decimal OldBudgetAmount { get; set; }
        public decimal NewBudgetAmount { get; set; }
        public string? Remarks { get; set; }
        public int CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string? CreatedIP { get; set; }
    }
}
