using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Mappings;

namespace BSOFT.Application.Companies.Queries.GetCompanyAutoComplete
{
    public class CompanyAutoCompleteVm : IMapFrom<Company>
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}