using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Power.FeederGroup.Queries.GetFeederGroupAutoComplete
{
    public class GetFeederGroupAutoCompleteQuery : IRequest<List<GetFeederGroupAutoCompleteDto>>

    {
         public string? SearchPattern { get; set; }
        
    }
}