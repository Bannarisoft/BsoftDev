
namespace Core.Application.DepreciationDetail.Queries.GetDepreciationDetail
{
    public class DepreciationCalculationDto
    {        
        public int companyId { get; set; }
        public int unitId { get; set; } 
        public string? finYear { get; set; } 
        public DateTimeOffset? startDate { get; set; }   
        public DateTimeOffset? endDate { get; set; }   
        public string? depreciationType { get; set; }
    }
}