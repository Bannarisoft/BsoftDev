
using Core.Application.Common.HttpResponse;
using Core.Application.WDVDepreciation.Queries.CalculateDepreciation;
using MediatR;

namespace Core.Application.WDVDepreciation.Commands.DeleteDepreciation
{
    public class DeleteDepreciationCommand  : IRequest<ApiResponseDTO<CalculationDepreciationDto>>
    {      
        public int FinYearId { get; set; }     
    }
}