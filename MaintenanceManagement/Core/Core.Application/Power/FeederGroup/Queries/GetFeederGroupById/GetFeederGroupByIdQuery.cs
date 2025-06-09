using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Power.FeederGroup.Queries.GetFeederGroup;
using MediatR;

namespace Core.Application.Power.FeederGroup.Queries.GetFeederGroupById
{
    public class GetFeederGroupByIdQuery :IRequest<ApiResponseDTO<GetFeederGroupByIdDto>>
    {

     public int  Id { get; set; }
        
    }
}