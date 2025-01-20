using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Departments.Queries.GetDepartments;
using MediatR;
using Core.Application.Common.Interfaces.IAdminSecuritySettings;
using AutoMapper;
using Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettings;

namespace Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettingsById
{
    public class GetAdminSecuritySettingsByIdQuery  :IRequest<Result<AdminSecuritySettingsDto>>
    {
       
       
    public int Id { get; set; }


    }
}