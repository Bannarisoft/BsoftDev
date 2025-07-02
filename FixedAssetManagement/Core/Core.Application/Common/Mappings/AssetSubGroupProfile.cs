using AutoMapper;
using Core.Application.AssetSubGroup.Command.CreateAssetSubGroup;
using Core.Application.AssetSubGroup.Command.DeleteAssetSubGroup;
using Core.Application.AssetSubGroup.Command.UpdateAssetSubGroup;
using Core.Application.AssetSubGroup.Queries.GetAssetSubGroup;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class AssetSubGroupProfile : Profile
    {
        public AssetSubGroupProfile()
        {
            CreateMap<Core.Domain.Entities.AssetSubGroup,AssetSubGroupDto>();
            CreateMap<CreateAssetSubGroupCommand, Core.Domain.Entities.AssetSubGroup>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.SubGroupName, opt => opt.MapFrom(src => src.SubGroupName))
                .ForMember(dest => dest.SortOrder, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

            CreateMap<UpdateAssetSubGroupCommand, Core.Domain.Entities.AssetSubGroup>()
                .ForMember(dest => dest.Code, opt => opt.Ignore())
                .ForMember(dest => dest.SubGroupName, opt => opt.MapFrom(src => src.SubGroupName))
                .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder))
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));

            CreateMap<DeleteAssetSubGroupCommand, Core.Domain.Entities.AssetSubGroup>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));               

            CreateMap<Core.Domain.Entities.AssetSubGroup, AssetSubGroupAutoCompleteDTO>(); 

                  
        }
    }
}
