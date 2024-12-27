using Core.Application.Entity.Queries.GetEntity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Entity.Queries.GetEntityById
{
    public class GetEntityByIdQuery : IRequest<EntityVm>
    {
        public int EntityId { get; set; }
    }
}