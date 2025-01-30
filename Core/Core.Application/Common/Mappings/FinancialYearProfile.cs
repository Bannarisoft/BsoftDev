using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.FinancialYear.Command.CreateFinancialYear;
using Core.Application.FinancialYear.Queries.GetFinancialYear;
using Core.Domain.Entities;
using Core.Domain.Enums;


namespace Core.Application.Common.Mappings
{
   

    public class FinancialYearProfile :Profile
    {
         public FinancialYearProfile()
        {
             CreateMap<Core.Domain.Entities.FinancialYear, FinancialYearDto>();

             CreateMap<CreateFinancialYearCommand, Core.Domain.Entities.FinancialYear>();
       
            
  

        }
    }
}