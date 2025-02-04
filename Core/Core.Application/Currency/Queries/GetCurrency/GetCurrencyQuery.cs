using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Currency.Queries.GetCurrency
{
    public class GetCurrencyQuery : IRequest<ApiResponseDTO<List<CurrencyDto>>>;       
    
}