using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.ShiftMasterDetails.Queries.GetShiftMasterDetailById
{
    public class GetShiftMasterByIdQuery : IRequest<ApiResponseDTO<ShiftMasterDetailByIdDto>>
    {
        public int Id { get; set; }
    }
}