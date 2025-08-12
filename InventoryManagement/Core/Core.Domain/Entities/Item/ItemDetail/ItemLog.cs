namespace Core.Domain.Entities.Item.ItemDetail
{
    public class ItemLog
    {
        public long Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string EntityName { get; set; } = null!;
        public int EntityId { get; set; }
        public string Action { get; set; } = "Update";
        public string ChangesJson { get; set; } = "{}"; // [{Property,Old,New}]
        public int? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedIP { get; set; }
        public string? CorrelationId { get; set; }
    }
}