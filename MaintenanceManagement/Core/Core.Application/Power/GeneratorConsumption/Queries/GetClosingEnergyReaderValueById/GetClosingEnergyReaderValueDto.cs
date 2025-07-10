using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Power.GeneratorConsumption.Queries.GetClosingEnergyReaderValueById
{
    public class GetClosingEnergyReaderValueDto
    {
        public int GeneratorId { get; set; }
        public int GeneratorCode { get; set; }
        public string? GeneratorName { get; set; }
        public decimal OpeningEnergyReading { get; set; }
    }
}