using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetGroup.Queries.GetAssetGroup;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetGroup.Queries.GetAssetGroupById
{
    public class GetAssetGroupByIdQuery : IRequest<ApiResponseDTO<AssetGroupDto>>
    {
        public int Id { get; set; }
    }
}