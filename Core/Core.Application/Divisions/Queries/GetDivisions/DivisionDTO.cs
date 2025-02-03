using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Core.Domain.Entities;
using Core.Application.Common.Mappings;
using Microsoft.AspNetCore.Http;
using Core.Application.Common;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Application.Divisions.Queries.GetDivisions
{
    public class DivisionDTO 
    {
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public Status IsActive { get; set; }
    }
}