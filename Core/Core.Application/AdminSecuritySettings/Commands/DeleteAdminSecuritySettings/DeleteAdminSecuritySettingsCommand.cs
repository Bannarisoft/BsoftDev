using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettings;
using MediatR;


namespace Core.Application.AdminSecuritySettings.Commands.DeleteAdminSecuritySettings
{
    public class DeleteAdminSecuritySettingsCommand : IRequest<Result<int>>
    {

         public int Id { get; set; }
        public AdminSecuritySettingsStatusDto AdminSecuritySettingsStatusDto { get; set; }
    }
}