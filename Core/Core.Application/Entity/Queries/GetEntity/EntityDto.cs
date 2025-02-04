using Core.Application.Common.HttpResponse;
using Core.Application.Common.Mappings;

namespace Core.Application.Entity.Queries.GetEntity
{
    public class EntityDto 
    {
    public int Id { get; set; }
    public string? EntityCode { get; set; }
    public string? EntityName { get; set; }
    public string? EntityDescription { get; set; }
    public string? Address { get; set; }
    public string? Phone  { get; set; }
    public string? Email { get; set; }
    }
}