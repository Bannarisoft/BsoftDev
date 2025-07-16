using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.ShiftMasters.Queries.GetShiftMaster;
using MediatR;

namespace Core.Application.ShiftMasters.Queries.GetShiftMasterById
{
    public class GetShiftMasterByIdQuery : IRequest<ApiResponseDTO<ShiftMasterDTO>>
    {
        public int Id { get; set; }
    }
}