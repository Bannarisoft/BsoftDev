namespace Core.Application.DepreciationDetail.Queries.GetDepreciationDetail
{
    public class DepreciationAbstractDto
    {
        public string? Company { get; set; }
        public string? Unit { get; set; } 
        public string? Division { get; set; } 
        public string? AssetGroup { get; set; }
        public decimal GrossBlockUpto { get; set; }
        public decimal Addition { get; set; }
        public decimal Deletion { get; set; }
        public decimal GrossBlockCurrent { get; set; }
        public decimal DepreciationUpto { get; set; }
        public decimal DepreciationCurrent { get; set; }
        public decimal DisposalDepreciation { get; set; }
        public decimal DepreciationAson { get; set; }
        public decimal NetBlockUpto { get; set; }
        public decimal NetBlock { get; set; }
        public DateTimeOffset startDate { get; set; }
        public DateTimeOffset endDate { get; set; }
        public string? DepType { get; set; }
        public string? DepPeriod { get; set; }
    }
}