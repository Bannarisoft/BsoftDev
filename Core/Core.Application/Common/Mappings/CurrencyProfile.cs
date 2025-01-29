using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Core.Application.Common.Mappings
{
    public class CurrencyProfile : Profile
    {
        public CurrencyProfile()
        {
            CreateMap<Core.Domain.Entities.Currency, Core.Application.Currency.Queries.GetCurrency.CurrencyDto>();
        }
    }
}