using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.DepreciationDetail.Queries.GetDepreciationMethod
{
    public class GetDepreciationMethodQuery  : IRequest<ApiResponseDTO<List<GetMiscMasterDto>>> 
    {
        
    }
}