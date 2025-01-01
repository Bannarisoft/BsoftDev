using AutoMapper;
using Core.Application.Entity.Queries.GetEntity;
using Core.Application.Entity.Commands.CreateEntity;
using Core.Application.Entity.Commands.UpdateEntity;
using Core.Application.Entity.Commands.DeleteEntity;
using Core.Domain.Entities;


namespace Core.Application.Common.Mappings
{
    public class EntityProfile : Profile
    {
       public EntityProfile()
        {
            CreateMap<Core.Domain.Entities.Entity, Core.Application.Entity.Queries.GetEntity.EntityDto>();
            CreateMap<Core.Application.Entity.Queries.GetEntity.EntityStatusDto, Core.Domain.Entities.Entity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
            CreateMap<CreateEntityCommand, Core.Domain.Entities.Entity>();
            CreateMap<UpdateEntityCommand, Core.Domain.Entities.Entity>();
            CreateMap<DeleteEntityCommand, Core.Domain.Entities.Entity>();            
        }

    }
}