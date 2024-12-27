using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Entity.Queries.GetEntity;
using MediatR;

namespace Core.Application.Entity.Queries.GetEntity
{
    public class GetEntityQuery : IRequest<List<EntityVm>>
    {
        
    }
}