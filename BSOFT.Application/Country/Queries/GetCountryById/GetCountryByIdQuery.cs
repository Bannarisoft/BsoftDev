using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Application.Country.DTO;
using MediatR;

namespace BSOFT.Application.Country.Queries
{
    public record GetCountryByIdQuery(int Id) : IRequest<CountryDto>;
    
}