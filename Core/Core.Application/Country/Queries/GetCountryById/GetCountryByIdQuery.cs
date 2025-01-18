using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using Core.Application.Country.Queries.GetCountries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Country.Queries.GetCountryById
{
    public class GetCountryByIdQuery : IRequest<ApiResponseDTO<CountryDto>>
    {
        public int Id { get; set; }
    }
}