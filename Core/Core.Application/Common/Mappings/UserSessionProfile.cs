using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.UserSession.Command;
using Core.Application.UserSession.Queries.GetUserSession;

namespace Core.Application.Common.Mappings
{
    public class UserSessionProfile : Profile
    {
        public UserSessionProfile()
        {
            CreateMap<Core.Domain.Entities.UserSession, Core.Application.UserSession.Queries.GetUserSession.UserSessionDto>();
            CreateMap<CreateUserSessionCommand, Core.Domain.Entities.UserSession>();
        }
    }
}   