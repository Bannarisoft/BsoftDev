using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Menu.Queries.GetMenuByModule
{
    public class GetMenuByModuleQuery : IRequest<ApiResponseDTO<List<MenuDTO>>>
    {
        public List<int>? ModuleId { get; set; }
    }
}