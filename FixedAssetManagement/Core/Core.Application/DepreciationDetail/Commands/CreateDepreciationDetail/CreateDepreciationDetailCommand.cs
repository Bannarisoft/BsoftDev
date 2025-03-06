
using Core.Application.Common.HttpResponse;
using Core.Application.DepreciationDetail.Queries.GetDepreciationDetail;
using MediatR;

namespace Core.Application.DepreciationDetail.Commands.CreateDepreciationDetail
{
    public class CreateDepreciationDetailCommand  : IRequest<ApiResponseDTO<DepreciationDto>>  
    {
        public int companyId { get; set; } 
        public int unitId { get; set; } 
        public string? finYear { get; set; }        
        public DateTimeOffset startDate { get; set; }    
        public DateTimeOffset endDate { get; set; }   
        public string? depreciationType { get; set; }                
    }
}