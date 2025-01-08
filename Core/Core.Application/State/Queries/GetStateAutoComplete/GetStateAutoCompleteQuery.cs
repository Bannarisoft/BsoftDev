using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.State.Queries.GetStates;
using MediatR;


namespace Core.Application.State.Queries.GetStateAutoComplete
{
    public class GetStateAutoCompleteQuery : IRequest<Result<List<StateDto>>>
    {
        public string SearchPattern { get; set; }=string.Empty;     
    }
}
