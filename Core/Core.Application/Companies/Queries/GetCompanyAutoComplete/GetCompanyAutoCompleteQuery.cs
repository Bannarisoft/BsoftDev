using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Companies.Queries.GetCompanies;
using MediatR;

namespace Core.Application.Companies.Queries.GetCompanyAutoComplete
{
    public class GetCompanyAutoCompleteQuery : IRequest<List<CompanyAutoCompleteDTO>>
    {
        
        public string SearchPattern { get; set; }
    }
}