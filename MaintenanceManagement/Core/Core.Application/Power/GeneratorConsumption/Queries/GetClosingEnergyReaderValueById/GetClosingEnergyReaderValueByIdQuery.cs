using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.Power.GeneratorConsumption.Queries.GetClosingEnergyReaderValueById
{
    public class GetClosingEnergyReaderValueByIdQuery :  IRequest<GetClosingEnergyReaderValueDto>
    {
        public int GeneratorId { get; set; }
    }
}