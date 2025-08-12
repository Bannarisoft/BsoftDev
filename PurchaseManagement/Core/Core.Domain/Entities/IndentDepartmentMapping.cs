using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class IndentDepartmentMapping
    {
        public int Id { get; set; }
        public int IndentHeaderId { get; set; }
        public int DepartmentId { get; set; }
        public IndentHeader IndentHeader { get; set; }
    }
}