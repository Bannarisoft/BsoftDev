// Core.Application/Item/Queries/ItemDetailsDto.cs  (READ model for GetById)

namespace Core.Application.Item.ItemDetail.Queries.GetAllItems
{
    public class ItemDetailsDto : ItemDto
    {
        public int Id { get; set; }                
        public string? HSNCode { get; set; }         // from HSNMaster
        public string? ItemGroupName { get; set; }   // from ItemGroup
    }
}
