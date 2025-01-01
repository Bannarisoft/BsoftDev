using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.UserRole.Commands.UpdateRole;
using Core.Domain.Entities;

namespace Core.Application.Common.Mappings
{
    public class UpdateUserRoleProfile :Profile
    {

     public UpdateUserRoleProfile()
    {
         CreateMap<UpdateRoleCommand, Core.Domain.Entities.UserRole>();
    }
    }
}