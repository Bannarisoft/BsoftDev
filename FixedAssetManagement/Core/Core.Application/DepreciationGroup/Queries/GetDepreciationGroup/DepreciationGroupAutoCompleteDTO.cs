using Core.Application.Common.Mappings;
using Core.Domain.Entities;

namespace Core.Application.DepreciationGroup.Queries.GetDepreciationGroup
{
    public class DepreciationGroupAutoCompleteDTO  : IMapFrom<DepreciationGroups>
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? DepreciationGroupName { get; set; } 
    }
}