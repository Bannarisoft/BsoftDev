using Core.Application.Common.Mappings;
using Core.Domain.Entities;

namespace Core.Application.SpecificationMaster.Queries.GetSpecificationMaster
{
    public class SpecificationMasterAutoCompleteDTO : IMapFrom<SpecificationMasters>
    {
        public int Id { get; set; }        
        public string? SpecificationName { get; set; } 
        public byte ISDefault { get; set; }

    }
}