namespace Core.Application.Item.ItemCategory.Queries.GetItemCategoryAutoComplete
{
    public class ItemCategoryAutoCompleteDto
    {
        public int Id { get; set; }
        public string? ItemCategoryName { get; set; }    
        public string? ParentCategoryName { get; set; }            
    }
}