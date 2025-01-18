using Core.Application.Common;
using Core.Application.Common.Mappings;

namespace Core.Application.Entity.Queries.GetEntity
{
    public class EntityDto :IMapFrom<Result <Core.Domain.Entities.Entity>>
    {
    public int Id { get; set; }
    public string EntityCode { get; set; }
    public string EntityName { get; set; }
    public string EntityDescription { get; set; }
    public string Address { get; set; }
    public string Phone  { get; set; }
    public string Email { get; set; }
    public byte IsActive { get; set; }
    }
}