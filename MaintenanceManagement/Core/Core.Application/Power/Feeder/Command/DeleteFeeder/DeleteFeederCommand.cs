using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Power.Feeder.Queries.GetFeeder;
using MediatR;

namespace Core.Application.Power.Feeder.Command.DeleteFeeder
{
    public class DeleteFeederCommand : IRequest<bool>
    {
         public int Id { get; set; }
    }
}