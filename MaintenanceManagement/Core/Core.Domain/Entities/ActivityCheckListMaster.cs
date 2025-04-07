using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class ActivityCheckListMaster : BaseEntity
    {

        public int   ActivityId { get; set; }

        public string? ActivityCheckList { get; set; }

     //   public Status? IsActive { get; set; }
         
        
         public ActivityMaster? ActivityMaster { get; set; }  

        
    }
}