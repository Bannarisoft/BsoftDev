using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetCustodian
{
    public class GetAssetCustodianQuery : IRequest<ApiResponseDTO<List<GetAssetCustodianDto>>>
    {
        public string? OldUnitId { get; set; }
        
        public int DepartmentId { get; set; }
    }
}