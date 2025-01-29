using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.FinancialYear.Queries.GetFinancialYear
{
    public class GetFinancialYearQuery  : IRequest<ApiResponseDTO<List<FinancialYearDto>>>
    {
       
        


    }
}