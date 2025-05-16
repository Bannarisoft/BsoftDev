using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Reports.MRS
{
    public class MRSReportDto
    {
        public string? DivCode { get; set; }              
        public string? IrNo { get; set; }                 
        public DateTime IrDate { get; set; }            
        public string? WoNo { get; set; }                
        public string? Remarks { get; set; }              
        public string? MaintainenceType { get; set; }    
        public string? DepartmentName { get; set; }       
        public string? SubDept { get; set; }  
        public int IrSno { get; set; }                  
        public string? ItemCode { get; set; }            
        public string? ItemName { get; set; }             
        public string? MachineNo { get; set; }            
        public decimal Qty { get; set; }                 
        public string? CategoryDescription { get; set; }  
        public string? SubCategoryName { get; set; }      
        public decimal Rate { get; set; }              
        public decimal Value { get; set; }
    }
}