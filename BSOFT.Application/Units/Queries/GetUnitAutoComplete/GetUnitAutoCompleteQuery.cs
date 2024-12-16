using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;


namespace BSOFT.Application.Units.Queries.GetUnitAutoComplete
{
    public class GetUnitAutoCompleteQuery : IRequest<List<UnitAutoCompleteVm>>
    {
        public string SearchPattern { get; set; }
    }
}