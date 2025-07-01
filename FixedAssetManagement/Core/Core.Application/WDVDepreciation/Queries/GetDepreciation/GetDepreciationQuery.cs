
using Core.Application.Common.HttpResponse;
using Core.Application.WDVDepreciation.Queries.CalculateDepreciation;
using MediatR;

namespace Core.Application.WDVDepreciation.Queries.GetDepreciation
{
    public class GetDepreciationQuery  : IRequest<ApiResponseDTO<List<CalculationDepreciationDto>>>
    {      
        public int FinYearId { get; set; }     
    }
}