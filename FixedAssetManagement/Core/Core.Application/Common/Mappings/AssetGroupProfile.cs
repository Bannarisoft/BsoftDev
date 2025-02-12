using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetGroup.Command.CreateAssetGroup;
using Core.Application.AssetGroup.Command.DeleteAssetGroup;
using Core.Application.AssetGroup.Command.UpdateAssetGroup;
using Core.Application.AssetGroup.Queries.GetAssetGroup;
using Core.Domain.Common;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class AssetGroupProfile : Profile
    {
        public AssetGroupProfile()
        {
            CreateMap<Core.Domain.Entities.AssetGroup,AssetGroupDto>();
            CreateMap<CreateAssetGroupCommand, Core.Domain.Entities.AssetGroup>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupName))
                .ForMember(dest => dest.SortOrder, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

            CreateMap<UpdateAssetGroupCommand, Core.Domain.Entities.AssetGroup>()
                .ForMember(dest => dest.Code, opt => opt.Ignore())
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupName))
                .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));

            CreateMap<DeleteAssetGroupCommand, Core.Domain.Entities.AssetGroup>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));    
           

            CreateMap<Core.Domain.Entities.AssetGroup, AssetGroupAutoCompleteDTO>(); 

                  
        }
    }
}
