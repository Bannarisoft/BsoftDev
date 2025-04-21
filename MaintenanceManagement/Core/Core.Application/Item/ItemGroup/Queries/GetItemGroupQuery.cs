using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Item.ItemGroup.Queries
{
    public class GetItemGroupQuery : IRequest<ApiResponseDTO<List<GetItemGroupDto>>>
    {
        public string? OldUnitId { get; set; } 
    }
}