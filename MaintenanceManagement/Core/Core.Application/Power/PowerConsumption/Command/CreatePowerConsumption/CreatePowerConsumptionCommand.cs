using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Power.PowerConsumption.Command.CreatePowerConsumption
{
    public class CreatePowerConsumptionCommand : IRequest<ApiResponseDTO<int>>
    {
        public int FeederTypeId { get; set; }
        public int FeederId { get; set; }
        public int UnitId { get; set; }
        public decimal OpeningReading { get; set; }
        public decimal ClosingReading { get; set; }
        
    }
}