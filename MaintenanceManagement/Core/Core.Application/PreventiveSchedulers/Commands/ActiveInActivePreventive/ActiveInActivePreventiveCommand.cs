using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Commands.ActiveInActivePreventive
{
    public class ActiveInActivePreventiveCommand : IRequest<ApiResponseDTO<bool>>
    {
        public int Id { get; set; }
        public byte IsActive { get; set; }
    }
}