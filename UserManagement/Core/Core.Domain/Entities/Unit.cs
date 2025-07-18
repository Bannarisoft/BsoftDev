using Core.Domain.Common;


namespace Core.Domain.Entities
{

public class Unit : BaseEntity
{
public int Id { get; set; }

public string? UnitName { get; set; }

public string? ShortName { get; set; }
public int CompanyId { get; set; }
public Company? Company { get; set; }
public int DivisionId { get; set; }
public Division? Division { get; set; }
public string? UnitHeadName { get; set; }
public string? CINNO {get; set;}
public string? OldUnitId { get; set;}
public  UnitAddress? UnitAddress { get; set; }
public  UnitContacts? UnitContacts { get; set; }
public IList<UserUnit>? UserUnits { get; set; }
public IList<CustomFieldUnit>? CustomFieldUnits { get; set; }
     
}
}