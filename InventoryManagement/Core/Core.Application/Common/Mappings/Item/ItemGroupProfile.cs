using AutoMapper;
using Core.Application.Item.ItemGroup.Commands.CreateItemGroup;
using Core.Application.Item.ItemGroup.Commands.DeleteItemGroup;
using Core.Application.Item.ItemGroup.Commands.UpdateItemGroup;
using Core.Application.Item.ItemGroup.Queries.GetItemGroup;
using Core.Application.Item.ItemGroup.Queries.GetItemGroupAutoComplete;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings.Item
{
    public class ItemGroupProfile : Profile
    {
        public ItemGroupProfile()
        {
            CreateMap<Domain.Entities.Item.ItemGroup, ItemGroupDto>();
            CreateMap<Domain.Entities.Item.ItemGroup, ItemGroupAutoCompleteDto>();
            CreateMap<CreateItemGroupCommand, Domain.Entities.Item.ItemGroup>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ItemGroupCode, opt => opt.MapFrom(src => src.ItemGroupCode))
                .ForMember(dest => dest.ItemGroupName, opt => opt.MapFrom(src => src.ItemGroupName))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));


            CreateMap<UpdateItemGroupCommand, Domain.Entities.Item.ItemGroup>()
                 .ForMember(dest => dest.ItemGroupCode, opt => opt.MapFrom(src => src.ItemGroupCode))
                .ForMember(dest => dest.ItemGroupName, opt => opt.MapFrom(src => src.ItemGroupName))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == 1 ? Status.Active : Status.Inactive));


            CreateMap<DeleteItemGroupCommand, Domain.Entities.Item.ItemGroup>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));

        }
    }
}