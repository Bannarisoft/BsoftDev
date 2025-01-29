using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.FinancialYear.Queries.GetFinancialYear;
using Core.Domain.Entities;


namespace Core.Application.Common.Mappings
{
     using FY = Core.Domain.Entities.FinancialYear;

    public class FinancialYearProfile :Profile
    {
               public FinancialYearProfile()
        {
             CreateMap<FY, FinancialYearDto>();

        }
    }
}