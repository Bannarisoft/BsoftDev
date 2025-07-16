using Core.Application.Common.Mappings;
using Core.Domain.Entities;

namespace Core.Application.Manufacture.Queries.GetManufacture
{
    public class ManufactureAutoCompleteDTO : IMapFrom<Manufactures>
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? ManufactureName { get; set; } 
    }
}