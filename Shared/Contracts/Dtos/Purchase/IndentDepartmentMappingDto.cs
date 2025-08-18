using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Dtos.Purchase
{
    public class IndentDepartmentMappingDto
    {
        public int Id { get; set; }
        public int IndentHeaderId { get; set; }
        public int DepartmentId { get; set; }
    }
}