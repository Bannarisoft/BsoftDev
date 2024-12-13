using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BSOFT.Application.Divisions.Queries.GetDivisionAutoComplete
{
    public class GetDivisionAutoCompleteQuery : IRequest<List<DivisionAutoCompleteVm>>
    {
        
        public string SearchPattern { get; set; }
    }
}