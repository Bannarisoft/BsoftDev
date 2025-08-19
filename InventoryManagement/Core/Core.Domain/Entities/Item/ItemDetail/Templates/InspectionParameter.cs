
using Core.Domain.Common;
using Core.Domain.Entities.Item.ItemDetail.Templates;

namespace Core.Domain.Entities.item.ItemDetail.Templates
{
    public class InspectionParameter : BaseEntity
    {
        public int TemplateId { get; set; }
        public InspectionTemplate Template { get; set; } = null!;
        public string Parameter { get; set; } = null!;
        public string? AcceptanceCriteriaValue { get; set; } 
        public bool Numeric { get; set; }
        public decimal? MinimumValue { get; set; }
        public decimal? MaximumValue { get; set; }
    }
}