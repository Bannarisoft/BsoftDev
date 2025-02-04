using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.TimeZones.Queries.GetTimeZones
{
    public class GetTimeZonesQuery : IRequest<ApiResponseDTO<List<TimeZonesDto>>>; 
        
    
}