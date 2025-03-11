using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class FormulaTable : BaseEntity
    {
        public string? FormulaName  { get; set; }
        public string? FormulaText  { get; set; }        
    }
}