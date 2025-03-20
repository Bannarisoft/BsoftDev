using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetParentMaster
{
    public class GetAssetParentMasterQuery : IRequest<ApiResponseDTO<List<AssetMasterGeneralAutoCompleteDTO>>>
    {
        public string? AssetType { get; set; }
    }
}