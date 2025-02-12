using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetCategories.Command.CreateAssetCategories;
using Core.Application.AssetCategories.Command.DeleteAssetCategories;
using Core.Application.AssetCategories.Command.UpdateAssetCategories;
using Core.Application.AssetCategories.Queries.GetAssetCategories;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class AssetCategoriesProfile : Profile
    {
        public AssetCategoriesProfile()
        {
             CreateMap<Core.Domain.Entities.AssetCategories,AssetCategoriesDto>();
             CreateMap<Core.Domain.Entities.AssetCategories, AssetCategoriesAutoCompleteDto>(); 
             CreateMap<CreateAssetCategoriesCommand, Core.Domain.Entities.AssetCategories>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.AssetGroupId, opt => opt.MapFrom(src => src.AssetGroupId))
                .ForMember(dest => dest.SortOrder, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

            CreateMap<UpdateAssetCategoriesCommand, Core.Domain.Entities.AssetCategories>()
                .ForMember(dest => dest.Code, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.AssetGroupId, opt => opt.MapFrom(src => src.AssetGroupId))
                .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));

            CreateMap<DeleteAssetCategoriesCommand, Core.Domain.Entities.AssetCategories>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted)); 
        }
    }
}