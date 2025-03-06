using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class DepreciationDetails : BaseEntity
    {        
        public int CompanyId { get; set; } 
        public int UnitId { get; set; } 
        public string? Finyear { get; set; }        
        public DateTimeOffset? StartDate { get; set; }    
        public DateTimeOffset? EndDate { get; set; }                    
        public string? DepreciationType { get; set; }
        public int? AssetId { get; set; }          
        public AssetMasterGenerals AssetMasterId { get; set; } = null!;      
        // Foreign Key
        public int AssetGroupId { get; set; }
        public AssetGroup AssetGroup { get; set; } = null!;    
        public decimal AssetValue { get; set; }
        public DateTimeOffset? CapitalizationDate { get; set; }                       
        public decimal ResidualValue { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }   
        public int UsefulLifeDays   { get; set; }   
        public int DaysOpening { get; set; }   
        public int DaysUsed { get; set; }   
        public decimal OpeningValue { get; set; }   
        public decimal DepreciationValue { get; set; }   
        public decimal ClosingValue { get; set; }   
        public byte IsLocked { get; set; }
    }
}