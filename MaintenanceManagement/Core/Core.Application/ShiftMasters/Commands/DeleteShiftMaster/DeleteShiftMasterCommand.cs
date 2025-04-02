using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.ShiftMasters.Commands.DeleteShiftMaster
{
    public class DeleteShiftMasterCommand : IRequest<ApiResponseDTO<bool>>
    {
        public int Id { get; set; }
    }
}