using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetDtlToTransfer;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetBulkAssetToTransfer
{
    public class GetBulkAssetToTransferQuery : IRequest<ApiResponseDTO<List<GetAssetDetailsToTransferHdrDto>>>
    {

        public int DepartmentId { get; set; }
        public string? CustodianId { get; set; }        
        public string?  CategoryID { get; set; }
        
    }
}