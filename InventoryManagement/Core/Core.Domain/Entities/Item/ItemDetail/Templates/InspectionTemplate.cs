

using Core.Domain.Common;
using Core.Domain.Entities.item.ItemDetail.Templates;

namespace Core.Domain.Entities.Item.ItemDetail.Templates
{
    public class InspectionTemplate : BaseEntity
    {
        public string TemplateName { get; set; } = null!;
        public ICollection<InspectionParameter> Parameters { get; set; } = new List<InspectionParameter>();        
        public ICollection<ItemQuality> Items { get; set; } = new List<ItemQuality>();
    }
}