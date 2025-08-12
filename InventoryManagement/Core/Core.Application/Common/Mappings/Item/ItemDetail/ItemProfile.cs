using AutoMapper;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using Core.Domain.Entities.Item.ItemDetail;

namespace Core.Application.Common.Mappings.Item.ItemDetail
{
   public sealed class ItemProfile : Profile
    {
        public ItemProfile()
        {
            // Create map: DTO -> Entity
            CreateMap<ItemDto, ItemMaster>()
                .ForMember(d => d.Id, o => o.Ignore()); // Id set by DB

            CreateMap<ItemPurchaseDto, ItemPurchase>();
            CreateMap<ItemInventoryDto, ItemInventory>();
            CreateMap<ItemQualityDto, ItemQuality>();

            // Update map: ignore nulls (so partial update doesn’t wipe values)
            CreateMap<ItemDto, ItemMaster>()
                .ForAllMembers(opt =>
                    opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ItemPurchaseDto, ItemPurchase>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ItemInventoryDto, ItemInventory>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ItemQualityDto, ItemQuality>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
