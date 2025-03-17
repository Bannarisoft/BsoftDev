using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.UserGroup.Queries.GetUserGroupAutoComplete;

namespace Core.Application.Common.Mappings 
{
    public class UserGroupProfile  : Profile
    {
        public UserGroupProfile()
        {        
            CreateMap<Core.Domain.Entities.UserGroup, UserGroupAutoCompleteDto>();
        }
    }
}