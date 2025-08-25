namespace Core.Application.Item.ItemDetail.Queries.GetItemAutoComplete
{
    public class GetItemAutoCompleteDto
    {
        public int Id { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }    
        public string? ParentItemId { get; set; }    
    }
}