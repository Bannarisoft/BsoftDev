using Core.Application.State.Queries.GetStates;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.State.Queries.GetStateById
{
    public class GetStateByIdQuery : IRequest<StateDto>
    {
        public int Id { get; set; }
    }
}