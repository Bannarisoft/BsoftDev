using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.RoleEntitlements.Queries.GetRoleEntitlements;
using MediatR;

namespace Core.Application.RoleEntitlements.Queries.GetRoleEntitlementById
{
    public class GetRoleEntitlementByIdQuery: IRequest<ApiResponseDTO<RoleEntitlementDto>>
    {
        public int Id { get; set; }
        
    }
}