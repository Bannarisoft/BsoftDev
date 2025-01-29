using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class  FinancialYear : BaseEntity
    {   
        public int Id { get; set; }
        public string StartYear { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 

        public string FinYearName { get; set; }

        public  byte IsActive  { get; set; }
    }
}