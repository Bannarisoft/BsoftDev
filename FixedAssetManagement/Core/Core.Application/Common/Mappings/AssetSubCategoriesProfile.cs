using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetSubCategories.Command.CreateAssetSubCategories;
using Core.Application.AssetSubCategories.Command.DeleteAssetSubCategories;
using Core.Application.AssetSubCategories.Command.UpdateAssetSubCategories;
using Core.Application.AssetSubCategories.Queries.GetAssetSubCategories;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class AssetSubCategoriesProfile :Profile
    {
        public AssetSubCategoriesProfile()
        {
             CreateMap<Core.Domain.Entities.AssetSubCategories,AssetSubCategoriesDto>();
             CreateMap<Core.Domain.Entities.AssetSubCategories, AssetSubCategoriesAutoCompleteDto>();
             CreateMap<CreateAssetSubCategoriesCommand, Core.Domain.Entities.AssetSubCategories>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(src => src.SubCategoryName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.AssetCategoriesId, opt => opt.MapFrom(src => src.AssetCategoriesId))
                .ForMember(dest => dest.SortOrder, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

            CreateMap<UpdateAssetSubCategoriesCommand, Core.Domain.Entities.AssetSubCategories>()
                .ForMember(dest => dest.Code, opt => opt.Ignore())
                .ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(src => src.SubCategoryName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.AssetCategoriesId, opt => opt.MapFrom(src => src.AssetCategoriesId))
                .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));

            CreateMap<DeleteAssetSubCategoriesCommand, Core.Domain.Entities.AssetSubCategories>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));  
        }
    }
}