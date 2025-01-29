using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;
using Core.Domain.Entities;

namespace Core.Application.FinancialYear.Command.CreateFinancialYear
{
    public class CreateFinancialYearCommand : IRequest<ApiResponseDTO<Core.Domain.Entities.FinancialYear>>
    {

        public int Id { get; set; }
        public string StartYear { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 

        public string FinYearName { get; set; }

        public  byte IsActive  { get; set; }
        
    }
}