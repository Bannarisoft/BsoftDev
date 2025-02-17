using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class UOM : BaseEntity
    {
        public string? Code { get; set; }
        public string? UOMName { get; set; }
        public int UOMType { get; set; }
        public int SortOrder { get; set; }
    }
}