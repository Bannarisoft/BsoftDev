using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;
using Core.Domain.Entities.WorkOrderMaster;

namespace Core.Domain.Entities
{
    public class ActivityCheckListMaster : BaseEntity
    {

        public int   ActivityId { get; set; }

        public string? ActivityCheckList { get; set; }

     //   public Status? IsActive { get; set; }
         
        
         public ActivityMaster? ActivityMaster { get; set; }  
         public ICollection<WorkOrderCheckList>? WOCheckLists {get; set;} 

        
    }
}