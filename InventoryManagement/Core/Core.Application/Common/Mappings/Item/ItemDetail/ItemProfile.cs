using AutoMapper;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using Core.Domain.Entities.Item.ItemDetail;
using Core.Domain.Entities.Item.ItemDetail.Variant;

namespace Core.Application.Common.Mappings.Item.ItemDetail
{
    public sealed class ItemProfile : Profile
    {
        public ItemProfile()
        {
            // ---------------- WRITE MAPS (DTO -> Entity) ----------------
            CreateMap<ItemDto, ItemMaster>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.ParentItemId,
                    o => o.MapFrom(src => src.ParentItemId > 0 ? src.ParentItemId : (int?)null))
                // ignore navs/collections — handled by dedicated repos
                .ForMember(d => d.ChildItems, o => o.Ignore())
                .ForMember(d => d.Purchase, o => o.Ignore())
                .ForMember(d => d.Inventory, o => o.Ignore())
                .ForMember(d => d.Quality, o => o.Ignore())
                .ForMember(d => d.VariantValues, o => o.Ignore())
                .ForMember(d => d.Suppliers, o => o.Ignore())
                .ForMember(d => d.Manufacture, o => o.Ignore())
                .ForMember(d => d.ItemUOMs, o => o.Ignore())
                .ForMember(d => d.HSNMaster, o => o.Ignore())
                .ForMember(d => d.ItemGroup, o => o.Ignore())
                .ForMember(d => d.ItemCategory, o => o.Ignore())
                .ForMember(d => d.UOM, o => o.Ignore())
                .ForMember(d => d.MiscClassification, o => o.Ignore())
                .ForMember(d => d.MiscStatus, o => o.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ItemPurchaseDto, ItemPurchase>()
                .ForMember(d => d.Id,     o => o.Ignore())   // <- IMPORTANT
                .ForMember(d => d.ItemId, o => o.Ignore())   // <- IMPORTANT
                .ForMember(d => d.Item,   o => o.Ignore())
                .ForMember(d => d.PurchaseUOM, o => o.Ignore());

           CreateMap<ItemInventoryDto, ItemInventory>()
                .ForMember(d => d.Id,     o => o.Ignore())
                .ForMember(d => d.ItemId, o => o.Ignore())
                .ForMember(d => d.Item,   o => o.Ignore())
                .ForMember(d => d.WeightUOM, o => o.Ignore())
                .ForMember(d => d.MiscDefaultMaterialRequestType, o => o.Ignore())
                .ForMember(d => d.MiscValuationMethod, o => o.Ignore())
                .ForMember(d => d.MiscRequestType, o => o.Ignore());


           CreateMap<ItemQualityDto, ItemQuality>()
                .ForMember(d => d.Id,     o => o.Ignore())
                .ForMember(d => d.ItemId, o => o.Ignore())
                .ForMember(d => d.Item,   o => o.Ignore())
                .ForMember(d => d.MiscCertificateType, o => o.Ignore()); // nav ignored


            CreateMap<ItemUomDto, ItemUOM>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Item, o => o.Ignore())
                .ForMember(d => d.ConversionUOM, o => o.Ignore());

            // ---------------- READ MAPS (Entity -> DTO) ----------------
            CreateMap<ItemMaster, ItemDto>()
                .ForMember(d => d.VariantValues, o => o.MapFrom(s => s.VariantValues));

            CreateMap<ItemMaster, ItemListDto>();

            CreateMap<ItemPurchase, ItemPurchaseDto>();
            CreateMap<ItemInventory, ItemInventoryDto>();
            CreateMap<ItemQuality, ItemQualityDto>();
            CreateMap<ItemUOM, ItemUomDto>();
            CreateMap<ItemSupplier, ItemSupplierDto>();
            CreateMap<ItemManufacture, ItemManufactureDto>();

            CreateMap<ItemVariantValue, VariantValueDto>()
                .ForMember(d => d.AttributeId, o => o.MapFrom(s => s.AttributeId))
                .ForMember(d => d.OptionValue, o => o.MapFrom(s => s.OptionValue));
        }
    }
}
