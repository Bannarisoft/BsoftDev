using Core.Application.RoleEntitlements.Queries.GetRoleEntitlements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Core.Application.Common.HttpResponse;

namespace Core.Application.RoleEntitlements.Commands.CreateRoleEntitlement
{
    public class CreateRoleEntitlementCommand : IRequest<ApiResponseDTO<int>>
    {
        
        public List<ModuleMenuPermissionDto> ModuleMenus { get; set; }
        

    }

}