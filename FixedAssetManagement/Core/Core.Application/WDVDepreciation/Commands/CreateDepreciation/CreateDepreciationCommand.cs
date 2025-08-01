
using Core.Application.Common.HttpResponse;
using Core.Application.WDVDepreciation.Queries.GetDepreciation;
using MediatR;

namespace Core.Application.WDVDepreciation.Commands.CreateDepreciation
{
    public class CreateDepreciationCommand  : IRequest<CalculationDepreciationDto>
    {      
        public int FinYearId { get; set; }     
    }
}