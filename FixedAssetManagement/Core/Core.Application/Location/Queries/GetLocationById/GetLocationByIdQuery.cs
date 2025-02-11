using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Location.Queries.GetLocations;
using MediatR;

namespace Core.Application.Location.Queries.GetLocationById
{
    public class GetLocationByIdQuery : IRequest<ApiResponseDTO<LocationDto>>
    {
        public int Id { get; set; }
        
    }
}