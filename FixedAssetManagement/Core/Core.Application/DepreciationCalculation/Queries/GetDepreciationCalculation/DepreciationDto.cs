
namespace Core.Application.DepreciationCalculation.Queries.GetDepreciationCalculation
{
    public class DepreciationDto 
    {
        public string? Company { get; set; }
        public string? Unit { get; set; } 
        public string? Division { get; set; } 
        public int AssetId { get; set; } 
        public string? AssetGroup { get; set; }
        public string? AssetCode { get; set; }        
        public string? AssetName { get; set; }                
        public int AssetValue { get; set; }   
        public int UsefulLife { get; set; }   
        public int Residual_Per { get; set; }   
        public int ResidualValue { get; set; }   
        public int UsefulLifeDays { get; set; }   
        public int DaysOpening { get; set; }   
        public DateTimeOffset ExpiryDate { get; set; }   
        public int DaysUsedCurrent { get; set; }   
        public int OpeningValue { get; set; }   
        public int DepreciationValue { get; set; }   
        public int ClosingValue { get; set; }   
    }
}