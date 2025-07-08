using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Power.FeederGroup.Queries.GetFeederGroup;
using MediatR;

namespace Core.Application.Power.FeederGroup.Command.DeleteFeederGroup
{
    public class DeleteFeederGroupCommand  : IRequest<bool>
    {
        
         public int Id { get; set; }
    }
}