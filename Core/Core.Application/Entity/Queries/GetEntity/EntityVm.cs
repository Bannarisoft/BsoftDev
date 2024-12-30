using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Application.Common.Mappings;
using Microsoft.AspNetCore.Http;
using Core.Application.Common;


namespace Core.Application.Entity.Queries.GetEntity
{
    public class EntityVm : BaseEntity,IMapFrom<Core.Domain.Entities.Entity>
    {
    public int EntityId { get; set; }
    public string EntityCode { get; set; }
    public string EntityName { get; set; }
    public string EntityDescription { get; set; }
    public string Address { get; set; }
    public string Phone  { get; set; }
    public string Email { get; set; }
    public byte IsActive { get; set; }
   
    }
}