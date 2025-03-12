using System;
using System.Collections.Generic;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetTranferIssueApproval.Commands.UpdateAssetTranferIssueApproval
{
    public class UpdateAssetTranferIssueApprovalCommand :IRequest<ApiResponseDTO<int>> 
    {
        public List<int>? Id { get; set; }
        public string Status { get; set; }

        public UpdateAssetTranferIssueApprovalCommand(List<int> id, string status)
        {
            Id = id;  // Corrected
            Status = status;
        }
    }
}