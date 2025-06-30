
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.WDVDepreciation.Queries.CalculateDepreciation
{
    public class CalculateDepreciationQuery  : IRequest<ApiResponseDTO<List<CalculationDepreciationDto>>>
    {      
        public int FinYearId { get; set; }     
    }
}