using Core.Application.Entity.Queries.GetEntity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Core.Application.Entity.Commands.UpdateEntity
{
    public class UpdateEntityCommand : IRequest<EntityDto>
    {
    public int EntityId { get; set; }
    public string EntityName { get; set; }
    public string EntityDescription { get; set; }
    public string Address { get; set; }
    public string Phone  { get; set; }
    public string Email { get; set; }
    public byte IsActive { get; set; }
    
    }
}