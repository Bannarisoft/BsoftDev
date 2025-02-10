using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class SubLocation : BaseEntity
    {
        public string? Code { get; set; }
        public string? SubLocationName { get; set; }
        public string? Description { get; set; }
        public int UnitId { get; set; }
        public int DepartmentId { get; set; }
        public Location? Location { get; set; }

    }
}