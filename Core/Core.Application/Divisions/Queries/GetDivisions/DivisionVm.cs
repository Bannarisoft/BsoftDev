using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Core.Domain.Entities;
using Core.Application.Common.Mappings;
using Microsoft.AspNetCore.Http;
using Core.Application.Common;

namespace Core.Application.Divisions.Queries.GetDivisions
{
    public class DivisionVm : BaseEntity,IMapFrom<Division>
    {
        public int DivId { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
    }
}