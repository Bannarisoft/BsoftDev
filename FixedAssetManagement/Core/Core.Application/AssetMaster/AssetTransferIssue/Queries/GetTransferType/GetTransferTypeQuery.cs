using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetTransferType
{
    public class GetTransferTypeQuery : IRequest<ApiResponseDTO<List<GetMiscMasterDto>>>
    {
      
        
    }
}