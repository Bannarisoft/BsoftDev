using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.ActivityMaster.Queries.GetActivityType
{
    public class GetActivityTypeQuery : IRequest<ApiResponseDTO<List<GetMiscMasterDto>>>
    {
        
    }
}