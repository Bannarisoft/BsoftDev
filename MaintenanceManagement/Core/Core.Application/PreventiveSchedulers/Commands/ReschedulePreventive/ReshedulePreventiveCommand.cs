using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Commands.ReschedulePreventive
{
    public class ReshedulePreventiveCommand : IRequest<ApiResponseDTO<bool>>
    {
        public int PreventiveScheduleDetailId { get; set; }
        public DateOnly RescheduleDate { get; set; }
    }
}