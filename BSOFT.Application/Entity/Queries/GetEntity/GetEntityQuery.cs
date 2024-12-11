using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Application.Entity.Queries.GetEntity;
using MediatR;

namespace BSOFT.Application.Entity.Queries.GetEntity
{
    public class GetEntityQuery : IRequest<List<EntityVm>>
    {
        
    }
}