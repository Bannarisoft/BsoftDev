using Core.Domain.Entities;
using Core.Application.Common.Mappings;

namespace Core.Application.Units.Queries.GetUnits
{
    public class UnitDto
    {
    public string UnitName { get; set; }
    public string ShortName { get; set; }
    public int CompanyId { get; set; }
    public int DivisionId { get; set; }
    public string UnitHeadName { get; set; }
    public string CINNO { get; set; }
    public byte IsActive { get; set; }
    public List<UnitAddressDto> UnitAddressDto { get; set; } = new List<UnitAddressDto>();
    public List<UnitContactsDto> UnitContactsDto { get; set;} = new List<UnitContactsDto>();
    }
    
}