using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Commands.MachineWiseFrequencyUpdate
{
    public class MachineWiseFrequencyUpdateCommand : IRequest<ApiResponseDTO<bool>>
    {
        public int Id { get; set; }
        public int FrequencyInterval { get; set; }
        public byte IsActive { get; set; }
    }
}