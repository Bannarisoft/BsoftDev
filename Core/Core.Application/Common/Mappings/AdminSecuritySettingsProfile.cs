using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Entities;
using Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettings;
using Core.Application.AdminSecuritySettings.Commands.CreateAdminSecuritySettings;
using Core.Application.AdminSecuritySettings.Commands.DeleteAdminSecuritySettings;
using Core.Application.AdminSecuritySettings.Commands.UpdateAdminSecuritySettings;

namespace Core.Application.Common.Mappings
{
    public class AdminSecuritySettingsProfile :Profile
    {
            public AdminSecuritySettingsProfile()
    {
            CreateMap <Core.Domain.Entities.AdminSecuritySettings, AdminSecuritySettingsDto>();

            CreateMap<CreateAdminSecuritySettingsCommand, Core.Domain.Entities.AdminSecuritySettings>();
              
            CreateMap<DeleteAdminSecuritySettingsCommand,  Core.Domain.Entities.AdminSecuritySettings>();

            CreateMap<UpdateAdminSecuritySettingsCommand, Core.Domain.Entities.AdminSecuritySettings>() ;

            CreateMap<AdminSecuritySettingsStatusDto, Core.Domain.Entities.AdminSecuritySettings>();
        
    }
        
    }
}