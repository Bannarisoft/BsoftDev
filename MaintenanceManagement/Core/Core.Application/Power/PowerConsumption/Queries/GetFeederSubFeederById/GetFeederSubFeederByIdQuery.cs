using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Power.PowerConsumption.Queries.GetFeederSubFeederById;
using MediatR;

namespace Core.Application.Power.PowerConsumption.Queries
{
    public class GetFeederSubFeederByIdQuery :  IRequest<List<GetFeederSubFeederDto>>
    {
        public int FeederTypeId { get; set; }
    }
}