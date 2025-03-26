using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTranferIssueApproval.Queries.GetAssetTransferIssueApproval;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetTranferIssueApproval.Queries.GetAssetTransferIssueById
{
    public class GetAssetTransferIssueByIdQuery : IRequest<ApiResponseDTO<List<AssetTransferIssueByIdDto>>>
    {
        public int Id {get; set;}
    }
}