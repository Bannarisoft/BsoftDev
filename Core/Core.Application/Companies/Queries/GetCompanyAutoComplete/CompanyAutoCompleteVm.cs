using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Application.Common.Mappings;

namespace Core.Application.Companies.Queries.GetCompanyAutoComplete
{
    public class CompanyAutoCompleteVm : IMapFrom<Company>
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}