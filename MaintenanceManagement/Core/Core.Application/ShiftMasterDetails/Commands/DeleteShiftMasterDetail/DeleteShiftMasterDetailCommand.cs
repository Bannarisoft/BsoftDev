using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.ShiftMasterDetails.Commands.DeleteShiftMasterDetail
{
    public class DeleteShiftMasterDetailCommand : IRequest<ApiResponseDTO<bool>>
    {
        public int Id { get; set; }
    }
}