using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BSOFT.Application.Companies.Queries.GetCompanyAutoComplete
{
    public class GetCompanyAutoCompleteQuery : IRequest<List<CompanyAutoCompleteVm>>
    {
        
        public string SearchPattern { get; set; }
    }
}