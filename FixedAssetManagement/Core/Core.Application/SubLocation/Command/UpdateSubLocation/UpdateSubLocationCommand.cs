using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Location.Command.UpdateSubLocation
{
    public class UpdateSubLocationCommand: IRequest<ApiResponseDTO<bool>>
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? SubLocationName { get; set; }
        public string? Description { get; set; }
        public int UnitId { get; set; }
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }
        public byte IsActive { get; set; }
    }
}