using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class MiscMaster : BaseEntity
    {
        public int MiscTypeId { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public MiscTypeMaster? MiscTypeMaster { get; set; }
        public ICollection<IndentHeader> IndentType { get; set; }
        public ICollection<IndentHeader> StatusHeader { get; set; }
        public ICollection<IndentDetail> StatusDetail { get; set; }
        

    }
}