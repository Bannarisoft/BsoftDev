
using Core.Application.Common.HttpResponse;
using Core.Application.WDVDepreciation.Queries.GetDepreciation;
using MediatR;

namespace Core.Application.WDVDepreciation.Commands.LockDepreciation
{
    public class LockDepreciationCommand  : IRequest<ApiResponseDTO<CalculationDepreciationDto>>
    {      
        public int FinYearId { get; set; }     
    }
}