using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.UOM.Queries.GetUOMs
{
    public class UOMDto
    {
        
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? UOMName { get; set; }
        public int SortOrder { get; set; }
        public int UOMTypeId { get; set; }
        public Status IsActive { get; set; }
    }
}