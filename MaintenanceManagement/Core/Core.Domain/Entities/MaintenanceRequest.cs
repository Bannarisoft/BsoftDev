using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class MaintenanceRequest  : BaseEntity
    {
       public int RequestTypeId  { get; set; } 
       public MiscMaster MiscRequestType { get; set; }
       public int MaintenanceTypeId  { get; set; }
       public MiscMaster MiscMaintenanceType { get; set; }

       public int MachineId { get; set; }

        // Navigation property to MachineMaster
       public MachineMaster Machine { get; set; } = null!;
       public int DepartmentId { get; set; }
        public int SourceId { get; set; }
       public int? VendorId  { get; set; }
        public string? OldVendorId  { get; set; }
       public string? Remarks { get; set; }
       


    }
}