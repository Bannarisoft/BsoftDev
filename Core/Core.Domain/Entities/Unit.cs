using Core.Domain.Common;

namespace Core.Domain.Entities
{

    public class Unit : BaseEntity
{
public int Id { get; set; }

public string UnitName { get; set; }

public string ShortName { get; set; }
public int CompanyId { get; set; }
public int DivisionId { get; set; }
public string UnitHeadName { get; set; }
public string CINNO {get; set;}
public List<UnitAddress> UnitAddress { get; set; }=new List<UnitAddress>();
public List<UnitContacts> UnitContacts { get; set; }=new List<UnitContacts>();
       
}
}