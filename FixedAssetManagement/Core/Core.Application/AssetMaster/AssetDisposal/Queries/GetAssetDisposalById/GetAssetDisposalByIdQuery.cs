using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetDisposal.Queries.GetAssetDisposal;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetDisposal.Queries.GetAssetDisposalById
{
    public class GetAssetDisposalByIdQuery : IRequest<ApiResponseDTO<AssetDisposalDto>>
    {
        public int Id { get; set; }
    }
}