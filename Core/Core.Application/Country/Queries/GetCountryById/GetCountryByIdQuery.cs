using Core.Application.Common;
using Core.Application.Country.Queries.GetCountries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Country.Queries.GetCountryById
{
    public class GetCountryByIdQuery : IRequest<Result<CountryDto>>
    {
        public int Id { get; set; }
    }
}