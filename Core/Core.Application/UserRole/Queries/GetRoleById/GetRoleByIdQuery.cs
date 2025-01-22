
using Core.Application.Common.HttpResponse;
using Core.Application.UserRole.Queries.GetRole;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.UserRole.Queries.GetRoleById
{
    public class GetRoleByIdQuery :IRequest<ApiResponseDTO<UserRoleDto>>
    {
      public int Id { get; set; }
    }
}