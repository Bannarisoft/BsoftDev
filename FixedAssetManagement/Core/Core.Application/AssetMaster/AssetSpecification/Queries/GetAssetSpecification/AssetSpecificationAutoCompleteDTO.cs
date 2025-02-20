using Core.Application.Common.Mappings;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification
{
    public class AssetSpecificationAutoCompleteDTO : IMapFrom<AssetSpecifications>
    {
        public int Id { get; set; }        
        public string? SpecificationName { get; set; } 
    }
}