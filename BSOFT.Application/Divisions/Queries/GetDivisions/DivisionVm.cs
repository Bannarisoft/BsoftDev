using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Mappings;
using Microsoft.AspNetCore.Http;
using BSOFT.Application.Common;

namespace BSOFT.Application.Divisions.Queries.GetDivisions
{
    public class DivisionVm : BaseEntityVm,IMapFrom<Division>
    {
        public int DivId { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
    }
}