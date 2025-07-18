using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.SubLocation.Queries.GetSubLocations;
using MediatR;

namespace Core.Application.SubLocation.Command.CreateSubLocation
{
    public class CreateSubLocationCommand : IRequest<ApiResponseDTO<SubLocationDto>>
    {
        public string? Code { get; set; }
        public string? SubLocationName { get; set; }
        public string? Description { get; set; }
        public int UnitId { get; set; }
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }

    }
}