using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Location.Queries.GetLocations;
using MediatR;

namespace Core.Application.Location.Command.CreateLocation
{
    public class CreateLocationCommand : IRequest<ApiResponseDTO<LocationDto>>
    {
        public string? Code { get; set; }
        public string? LocationName { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public int UnitId { get; set; }
        public int DepartmentId { get; set; }

    }
}