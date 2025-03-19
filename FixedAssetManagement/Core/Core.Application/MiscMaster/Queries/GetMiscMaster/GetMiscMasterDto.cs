using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.MiscMaster.Queries.GetMiscMaster
{
    public class GetMiscMasterDto
    {
        public int Id { get; set; }
        public int MiscTypeId { get; set; }  
        public string? Code { get; set;}
        public string? Description { get; set;}
        public int SortOrder  { get; set;}
        public Status  IsActive { get; set; }
        public IsDelete IsDeleted { get; set; }
    }
}