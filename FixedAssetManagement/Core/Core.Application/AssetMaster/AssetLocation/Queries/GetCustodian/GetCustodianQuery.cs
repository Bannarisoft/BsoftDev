using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetLocation.Queries.GetCustodian
{
    public class GetCustodianQuery : IRequest<ApiResponseDTO<List<GetCustodianDto>>>
    {
        public int OldUnitId { get; set; }
        public int AssetSourceId { get; set; } 
        public string? SearchEmployee { get; set; }
        
    }
}