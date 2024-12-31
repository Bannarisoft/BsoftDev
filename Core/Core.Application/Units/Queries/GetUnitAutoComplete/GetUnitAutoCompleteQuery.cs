using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Units.Queries.GetUnits;
using MediatR;


namespace Core.Application.Units.Queries.GetUnitAutoComplete
{
    public class GetUnitAutoCompleteQuery : IRequest<List<UnitDto>>
    {
        public string SearchPattern { get; set; }
    }
}