using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.ShiftMasterCQRS.Queries.GetShiftMaster;
using MediatR;

namespace Core.Application.ShiftMasterCQRS.Queries.GetShiftMasterById
{
    public class GetShiftMasterByIdQuery : IRequest<ApiResponseDTO<ShiftMasterDTO>>
    {
        public int Id { get; set; }
    }
}