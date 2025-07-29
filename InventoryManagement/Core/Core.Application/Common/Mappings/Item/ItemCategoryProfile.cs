using AutoMapper;
using Core.Application.Item.ItemCategory.Commands.CreateItemCategory;
using Core.Application.Item.ItemCategory.Commands.DeleteItemCategory;
using Core.Application.Item.ItemCategory.Commands.UpdateItemCategory;
using Core.Application.Item.ItemCategory.Queries.GetItemCategory;
using Core.Application.Item.ItemCategory.Queries.GetItemCategoryAutoComplete;
using static Core.Domain.Common.BaseEntity;


namespace Core.Application.Common.Mappings.Item
{
    public class ItemCategoryProfile : Profile
    {
        public ItemCategoryProfile()
        {
           CreateMap<Domain.Entities.Item.ItemCategory,ItemCategoryDto>();
           CreateMap<Domain.Entities.Item.ItemCategory, ItemCategoryAutoCompleteDto>();
            CreateMap<CreateItemCategoryCommand, Domain.Entities.Item.ItemCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ItemGroupId, opt => opt.MapFrom(src => src.ItemGroupId))
                .ForMember(dest => dest.ItemCategoryName, opt => opt.MapFrom(src => src.ItemCategoryName))
                .ForMember(dest => dest.IsGroup, opt => opt.MapFrom(src => src.IsGroup))
                .ForMember(dest => dest.ParentCategoryId, opt => opt.MapFrom(src => src.ParentCategoryId)) 
                .ForMember(dest => dest.IsBudgetApplicable, opt => opt.MapFrom(src => src.IsBudgetApplicable)) 
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));


            CreateMap<UpdateItemCategoryCommand, Domain.Entities.Item.ItemCategory>()
                .ForMember(dest => dest.ItemGroupId, opt => opt.MapFrom(src => src.ItemGroupId))
                .ForMember(dest => dest.ItemCategoryName, opt => opt.MapFrom(src => src.ItemCategoryName))
                .ForMember(dest => dest.IsGroup, opt => opt.MapFrom(src => src.IsGroup))
                .ForMember(dest => dest.ParentCategoryId, opt => opt.MapFrom(src => src.ParentCategoryId)) 
                .ForMember(dest => dest.IsBudgetApplicable, opt => opt.MapFrom(src => src.IsBudgetApplicable))    
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));


              CreateMap<DeleteItemCategoryCommand, Domain.Entities.Item.ItemCategory>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));    
        }
    }
}