using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Divisions.Queries.GetDivisions;
using MediatR;

namespace Core.Application.Divisions.Queries.GetDivisionAutoComplete
{
    public class GetDivisionAutoCompleteQuery : IRequest<List<DivisionAutoCompleteDTO>>
    {
        
        public string SearchPattern { get; set; }
    }
}