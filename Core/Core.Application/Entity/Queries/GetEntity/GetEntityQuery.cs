using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Entity.Queries.GetEntity;
using MediatR;
using Core.Application.Common.Mappings;
using Core.Domain.Entities;
using Core.Application.Common;

namespace Core.Application.Entity.Queries.GetEntity
{
public class GetEntityQuery : IRequest<List<EntityDto>>;  
}