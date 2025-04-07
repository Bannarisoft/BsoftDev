using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.ActivityCheckListMaster.Queries.GetActivityCheckListMasterById
{
    public class GetActivityCheckListMasterByIdDto
    {
        
      public int ChecklistId { get; set; }
      public int ActivityID { get; set; }
      public string? ActivityChecklist { get; set; }
      public Status IsActive { get; set; } 
    }
}