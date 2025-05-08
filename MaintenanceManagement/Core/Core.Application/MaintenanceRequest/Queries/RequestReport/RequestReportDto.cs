using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.MaintenanceRequest.Queries.RequestReport
{
    public class RequestReportDto
    {
        public int RequestId { get; set; } 
        public int  UnitId { get; set; }
        public DateTimeOffset RequestDate { get; set; }
        public string? CreatedBy { get; set; }
        public int? DepartmentId { get; set; }
        public int? MachineId { get; set; }
        public int? MaintenanceTypeId { get; set; }   
        public string? MaintenanceType { get; set; }          
        public int? WorkOrderId { get; set; }
        public int? StatusId { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public string? RequestMinutesDifference { get; set; }
        public string? DownTime { get; set; }
        public string? TimeTakenToRepair { get; set; }

       

                


           
           

        






    }
}