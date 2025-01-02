using AutoMapper;
using Core.Application.State.Commands.CreateState;
using Core.Application.State.Commands;
using Core.Application.State.Queries.GetStates;
using Core.Domain.Entities;
using Core.Application.State.Commands.UpdateState;
using Core.Application.City.Commands.DeleteCity;

namespace Core.Application.Common.Mappings
{
    public class StateProfile : Profile
    {
        
        public StateProfile()
        {
            CreateMap<CreateStateCommand, States>();            
            CreateMap<UpdateStateCommand, States>();
            CreateMap<DeleteCityCommand, StateDto>();  
            CreateMap<States, StateDto>();          
        }
    }
}    
