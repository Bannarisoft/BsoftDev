using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Currency.Commands.CreateCurrency;
using Core.Application.Currency.Commands.DeleteCurrency;
using Core.Application.Currency.Commands.UpdateCurrency;

namespace Core.Application.Common.Mappings
{
    public class CurrencyProfile : Profile
    {
        public CurrencyProfile()
        {
            CreateMap<Core.Domain.Entities.Currency, Core.Application.Currency.Queries.GetCurrency.CurrencyDto>();
            CreateMap<CreateCurrencyCommand, Core.Domain.Entities.Currency>();
            CreateMap<UpdateCurrencyCommand, Core.Domain.Entities.Currency>();
            CreateMap<DeleteCurrencyCommand, Core.Domain.Entities.Currency>();

        }
    }
}