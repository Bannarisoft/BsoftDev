using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Power.PowerConsumption.Queries.GetPowerConsumption;
using MediatR;

namespace Core.Application.Power.PowerConsumption.Queries.GetPowerConsumptionById
{
    public class GetPowerConsumptionByIdQuery :IRequest<ApiResponseDTO<GetPowerConsumptionDto>>
    {
        public int Id { get; set; }
    }
}