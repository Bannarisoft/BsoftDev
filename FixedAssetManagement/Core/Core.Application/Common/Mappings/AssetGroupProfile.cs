using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetGroup.Command.CreateAssetGroup;
using Core.Application.AssetGroup.Queries.GetAssetGroup;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class AssetGroupProfile : Profile
    {
        public AssetGroupProfile()
        {
                CreateMap<Core.Domain.Entities.AssetGroup,AssetGroupDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
                
                CreateMap<CreateAssetGroupCommand, Core.Domain.Entities.AssetGroup>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.SortOrder, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));
        }
    }
}