using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.Common.HttpResponse;
using Core.Domain.Entities.AssetMaster;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Command.CreateAssetTransferIssue
{
    public class CreateAssetTransferIssueCommand   : IRequest<ApiResponseDTO<int>> 
    {

      public AssetTransferIssueHdrDto? AssetTransferIssueHdrDto { get; set; }
    }
}