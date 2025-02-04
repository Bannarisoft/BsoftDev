using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Application.FinancialYear.Queries.GetFinancialYear
{
    public class GetFinancialYearDto
    {
         public int Id { get; set; }
        public string StartYear { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 

        public string FinYearName { get; set; }

        public Status  IsActive { get; set; }
        public IsDelete IsDeleted { get; set; }
    }
}