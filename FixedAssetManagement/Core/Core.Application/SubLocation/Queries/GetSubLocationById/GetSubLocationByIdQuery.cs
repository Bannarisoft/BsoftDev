using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.SubLocation.Queries.GetSubLocations;
using MediatR;

namespace Core.Application.SubLocation.Queries.GetSubLocationById
{
    public class GetSubLocationByIdQuery : IRequest<ApiResponseDTO<SubLocationDto>>
    {
        public int Id { get; set; }
        
    }
}