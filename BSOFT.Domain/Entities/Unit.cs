using System.ComponentModel.DataAnnotations.Schema;
using BSOFT.Domain.Common;

namespace BSOFT.Domain.Entities
{

    public class Unit : BaseEntity
    {
    public int Id { get; set; }

public string UnitName { get; set; }

public string ShortName { get; set; }
public int CompanyId { get; set; }
public int DivisionId { get; set; }

public string UnitHeadName { get; set; }
public new byte IsActive { get; set; }
public ICollection<UnitAddress> UnitAddress { get; set; }
public ICollection<UnitContacts> UnitContacts { get; set; }
       
    }
}