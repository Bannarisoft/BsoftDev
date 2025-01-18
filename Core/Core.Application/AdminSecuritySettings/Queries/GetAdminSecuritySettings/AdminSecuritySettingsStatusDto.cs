using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common;
using MediatR;

namespace Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettings
{
    public class AdminSecuritySettingsStatusDto  :IRequest <Result<int>>
    {
          public byte IsActive { get; set; }

    }
}